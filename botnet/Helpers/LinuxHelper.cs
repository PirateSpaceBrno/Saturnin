using System.Diagnostics;
using System.Threading.Tasks;

namespace Saturnin.Helpers
{
    public static class LinuxHelper
    {
        private static async Task<string> ExecuteBashCommand(string command)
        {
            // according to: https://stackoverflow.com/a/15262019/637142
            // thanks to this we will pass everything as one command
            command = command.Replace("\"", "\"\"");

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"" + command + "\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            proc.WaitForExit();

            return await proc.StandardOutput.ReadToEndAsync();
        }
    }
}
