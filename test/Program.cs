using HlLib.VersionControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    public class Program
    {
        static void Main(string[] args)
        {
            var git = new Git(@"D:\home\Documents\Visual Studio 2015\Projects\HlLib");

            foreach (GitCommit commit in git.QueryCommits())
            {
                Console.WriteLine(commit);
                foreach (var diff in commit.QueryFileUpdates())
                {
                    Console.WriteLine("\t" + diff);
                }
            }
            foreach (var status in git.QueryFileStatuses())
            {
                Console.WriteLine(status);
            }

            Console.Read();
        }
    }
}
