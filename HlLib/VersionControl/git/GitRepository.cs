using HlLib.Diagnostics;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;

namespace HlLib.VersionControl
{
    public class GitRepository : IRepository
    {
        public string HeadBranch { get { return _repo.Head.CanonicalName; } }

        public GitRepository(string path)
        {
            _repo = new Repository(path);
        }

        #region IRepository
        public void SetCredentials(string username, string email, string password)
        {
            _username = username;
            _email = email;
            _password = password;
        }

        public IEnumerable<FileStatus> QueryFileStatuses()
        {
            foreach (var entry in _repo.RetrieveStatus())
            {
                var state = GitFileStatus.GetState(entry);
                if (state != State.Unknown)
                {
                    yield return new GitFileStatus(state, entry.FilePath);
                }
            }
        }

        public IEnumerable<Commit> QueryCommits()
        {
            return _repo.Commits
                .QueryBy(new CommitFilter()
                {
                    SortBy = CommitSortStrategies.Time,
                })
                .Select(commit => new GitCommit(commit, _repo));
        }

        public FileUpdateResult UpdateLocalFiles()
        {
            // pull
            var options = new PullOptions()
            {
                FetchOptions = new FetchOptions()
                {
                    CredentialsProvider = CreateCredentials(),
                },
            };

            var result = _repo.Network.Pull(CreateSignature(), options);
            return new GitFileUpdateResult(result, _repo);
        }

        public bool AddFiles(params string[] paths)
        {
            // add
            foreach (var path in paths)
            {
                _repo.Index.Add(path);
            }
            return true;
        }

        public FileUpdateResult CommitChanges(string message, params string[] paths)
        {
            // commit
            if (paths.Length == 0)
            {
                return new GitFileUpdateResult();
            }

            foreach (var path in paths)
            {
                _repo.Stage(path);
            }

            var author = CreateSignature();
            var commiter = author;

            var result = _repo.Commit(message, author, commiter);
            return new GitFileUpdateResult(result, _repo);
        }

        public bool UndoChanges(params string[] paths)
        {
            // checkout [files]
            if (paths.Length == 0)
            {
                return true;
            }

            _repo.CheckoutPaths(_repo.Head.CanonicalName, paths);
            return false;
        }
        #endregion

        public IEnumerable<string> QueryLocalBranches()
        {
            return _repo.Branches.Where(branchObj => !branchObj.IsRemote)
                .Select(branchObj => branchObj.CanonicalName);
        }

        public IEnumerable<string> QueryRemoteBranches()
        {
            return _repo.Branches.Where(branchObj => branchObj.IsRemote)
                .Select(branchObj => branchObj.CanonicalName);
        }

        public void CreateLocalBranch(string branch)
        {
            _repo.CreateBranch(branch);
        }

        public void DeleteLocalBranch(string branch)
        {
            _repo.Branches.Remove(branch);
        }

        public void Checkout(string branch, bool createOnLocal = false)
        {
            // checkout [branch]
            var branchObj = (createOnLocal) ?
                _repo.CreateBranch(branch) : _repo.Branches[branch];
            branchObj = _repo.Checkout(branchObj);
        }

        public void Push(string remote = "origin", string sourceBranch = null)
        {
            // push
            var remoteObj = _repo.Network.Remotes[remote];

            var options = new PushOptions()
            {
                CredentialsProvider = CreateCredentials(),
            };

            _repo.Network.Push(remoteObj, sourceBranch, options);
        }

        public static int Execute(params string[] args)
        {
            return Process.OpenSync(new Process.OpenParam()
            {
                Command = "git.exe",
                Arguments = args,
            });
        }

        private Signature CreateSignature()
        {
            return new Signature(_username, _email, new DateTimeOffset(DateTime.Now));
        }

        private CredentialsHandler CreateCredentials()
        {
            return (url, usernameFromUrl, types) => new SecureUsernamePasswordCredentials()
            {
                Username = _username,
                Password = new SecureString().SetString(_password),
            };
        }

        #region IDisposable
        private bool _disposed;
        private object _disposingLock = new object();

        ~GitRepository()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            lock(_disposingLock)
            {
                if (_disposed)
                {
                    return;
                }
                _disposed = true;

                if (disposing)
                {
                    if (_repo != null)
                    {
                        _repo.Dispose();
                    }
                }
            }
        }
        #endregion

        private Repository _repo;
        private string _username;
        private string _email;
        private string _password;
    }
}
