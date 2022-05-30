using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DiscordMusicBot
{
    public static class LogUtils
    {
        /// <summary>
        /// Write to console and save to log file
        /// </summary>
        /// <param name="log">Text output</param>
        /// <param name="logPath">Output file</param>
        /// <returns></returns>
        public static async Task LogAsync(string log, string logPath = null)
        {
            Console.WriteLine(log);
            if (!string.IsNullOrEmpty(logPath)) await File.AppendAllTextAsync(logPath, log + '\n');
        }

        /// <summary>
        /// Write to console and save to log file
        /// </summary>
        /// <param name="log">Text output</param>
        /// <param name="logPath">Output file</param>
        /// <returns></returns>
        public static void Log(string log, string logPath = null)
        {
            Console.WriteLine(log);
            if (!string.IsNullOrEmpty(logPath)) File.AppendAllText(logPath, log + '\n');
        }

        /// <summary>
        /// Clear out a log file
        /// </summary>
        /// <param name="logPath">Output file</param>
        /// <returns></returns>
        public static async Task ClearLogAsync(string logPath)
        {
            await File.WriteAllTextAsync(logPath, string.Empty);
        }

        /// <summary>
        /// Clear out a log file
        /// </summary>
        /// <param name="logPath">Output file</param>
        /// <returns></returns>
        public static async Task ClearLog(string logPath)
        {
            File.WriteAllText(logPath, string.Empty);
        }
    }
}
