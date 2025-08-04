using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagement.Models;

namespace ProjectManagement.Models
{
    public enum ProjectActivityStatus
    {
        Upcoming = 1,
        Started = 2,
        Ongoing = 3,
        Completed = 4,
        Incomplete = 5,
        OnHold = 6, // Changed from On-Hold for C# naming conventions
        SentForReview = 7 // Changed from Sent for Review
    }

    public static class StatusPriorities
    {
        // Lower number = higher priority (more critical/overriding)
        public static Dictionary<ProjectActivityStatus, int> PriorityMap = new Dictionary<ProjectActivityStatus, int>
        {
            { ProjectActivityStatus.Incomplete, 1 },    // Highest priority for negative status
            { ProjectActivityStatus.OnHold, 2 },
            { ProjectActivityStatus.Ongoing, 3 },       // Active work, but less critical than a blocker
            { ProjectActivityStatus.SentForReview, 4 }, // Waiting
            { ProjectActivityStatus.Started, 5 },       // Just started
            { ProjectActivityStatus.Upcoming, 6 },      // Not yet started
            { ProjectActivityStatus.Completed, 7 }      // Lowest priority if others exist, but ideal final state
        };
    }
    
}