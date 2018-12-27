using Tenders.Core.Helpers;
using Tenders.Integration.API.JobSeeker.Interfaces;

namespace Tenders.Integration.API.JobSeeker.Services
{
    public class JobSeekerActionsService : IJobSeekerActionsService
    {
        public string RunJob(string job)
        {
            return $@"cat <<EOF | kubectl create -f -
{job}
EOF"
            .Bash();
        }
    }
}
