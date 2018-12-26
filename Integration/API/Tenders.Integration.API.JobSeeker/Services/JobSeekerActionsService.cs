using System.Diagnostics;
using Tenders.Integration.API.JobSeeker.Interfaces;

namespace Tenders.Integration.API.JobSeeker.Services
{
    public class JobSeekerActionsService : IJobSeekerActionsService
    {
        public string RunJob(string job)
        {
            return _bash($@"cat <<EOF | kubectl create -f -
{job}
EOF");
        }

        private string _bash(string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }

    }
}
