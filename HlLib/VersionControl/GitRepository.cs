using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;

namespace HlLib.VersionControl
{
    public class GitRepository : IRepository
    {
        public GitRepository(string path)
        {
            _repo = new Repository(path);
        }

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
                    CredentialsProvider = (url, usernameFromUrl, types) => new SecureUsernamePasswordCredentials()
                    {
                        Username = _username,
                        Password = new SecureString().SetString(_password),
                    },
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
            return false;
        }

        public bool Checkout(string branch)
        {
            // checkout [branch]
            return false;
        }

        public bool Push(string destBranch, string sourceBranch = null)
        {
            // push
            return false;
        }

        private Signature CreateSignature()
        {
            return new Signature(_username, _email, new DateTimeOffset(DateTime.Now));
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
