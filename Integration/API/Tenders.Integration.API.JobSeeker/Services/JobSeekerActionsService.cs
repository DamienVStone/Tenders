using System;
using Tenders.Core.Helpers;
using Tenders.Integration.API.JobSeeker.Interfaces;

namespace Tenders.Integration.API.JobSeeker.Services
{
    public class JobSeekerActionsService : IJobSeekerActionsService
    {
        public string RunJob(string job)
        {
            var command = $@"cat <<EOF | kubectl create -f -
{job}
EOF";
            Console.WriteLine(command);
            return command
            .Bash();
        }
    }
}
