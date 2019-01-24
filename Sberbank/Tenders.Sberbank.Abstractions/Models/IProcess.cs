using System;
using Tenders.Core.Models;

namespace Tenders.Sberbank.Abstractions.Models
{
    public interface IProcess
    {
        DateTime Started { get; set; }
        DateTime? Ended { get; set; }
        ProcessState ProcessState { get; set; }
    }
}
