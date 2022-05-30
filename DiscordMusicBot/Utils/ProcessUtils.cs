using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

namespace DiscordMusicBot
{
    /// <summary>
    /// Process utilities
    /// </summary>
    public static class ProcessUtils
    {
        #region Extension
        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        /// <param name="cancellationToken"></param>
        public static Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (process.HasExited) return Task.CompletedTask;

            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) => tcs.TrySetResult(null);
            if (cancellationToken != default(CancellationToken)) cancellationToken.Register(() => tcs.TrySetCanceled());
            return process.HasExited ? Task.CompletedTask : tcs.Task;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processPath"></param>
        /// <param name="arg"></param>
        public static Process RunProcess(string processPath, string args)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = processPath,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            return Process.Start(processStartInfo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        public static Process RunCmdProcess(string args)
        {
            string filename = "cmd.exe";
            string contents = "/C";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                filename = "cmd.exe";
                contents = "/C";

            }
            else
            {
                filename = "bash";
                contents = "-c";
            }
            contents += $" \"{args}\"";

            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = filename,
                Arguments = contents,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            return Process.Start(processStartInfo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        public static async Task<string> GetProcessOutput(Process process)
        {
            await process.WaitForExitAsync();
            return await process.StandardOutput.ReadLineAsync();
        }
    }
}
