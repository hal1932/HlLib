using System;
using System.Text;
using System.Threading.Tasks;

namespace HlLib.Diagnostics
{
    public static class Process
    {
        public class OpenParam
        {
            public string Command { get; set; }
            public string[] Arguments { get; set; }

            public Action<string> OnOutputReceived { get; set; }
            public Action<string> OnErrorReceived { get; set; }
            public Func<string> OnInputRequired { get; set; }
            public Action<int> OnExit { get; set; }

            public bool CreateWindow { get; set; }
            public object Environment { get; set; }
            public string WorkingDirectory { get; set; }
        }

        public static System.Diagnostics.Process Open(OpenParam param)
        {
            var args = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = param.Command,
                Arguments = string.Join(" ", param.Arguments),
                CreateNoWindow = !param.CreateWindow,
                RedirectStandardOutput = (param.OnOutputReceived != null),
                RedirectStandardError = (param.OnErrorReceived != null),
                RedirectStandardInput = (param.OnInputRequired != null),
                UseShellExecute = param.CreateWindow,
                WorkingDirectory = param.WorkingDirectory,
            };

            if (param.Environment != null)
            {
                foreach (var env in param.Environment.ToDictionary())
                {
                    args.EnvironmentVariables[env.Key] = env.Value as string;
                }
            }

            if (param.OnOutputReceived != null)
            {
                args.StandardOutputEncoding = Encoding.UTF8;
                args.UseShellExecute = false;
            }
            if (param.OnErrorReceived != null)
            {
                args.StandardErrorEncoding = Encoding.UTF8;
                args.UseShellExecute = false;
            }

            var process = new System.Diagnostics.Process();
            process.StartInfo = args;

            if (param.OnExit != null)
            {
                process.EnableRaisingEvents = true;
                process.Exited += (sender, e) =>
                {
                    param.OnExit(process.ExitCode);
                };
            }

            return process;
        }

        public static async Task<int> OpenAsync(OpenParam param)
        {
            return await Task.Run(() =>
            {
                var process = Open(param);
                process.WaitForExit();
                return process.ExitCode;
            });
        }

        public static int OpenSync(OpenParam param)
        {
            var process = Open(param);
            process.WaitForExit();
            return process.ExitCode;
        }
    }
}
