using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagement.Models;
using ProjectManagement.Interfaces;

namespace ProjectManagement.Services
{
    public class ProjectService: IProjectAction
    {
        // This method calculates the project status based on its activities
        public async Task<int> GetProjectDisplayStatus(Project project)
        {
            // Rule 1: If the project has no activities, use its manually set status.
            if (project.Activities == null || !project.Activities.Any())
            {
                return project.ProjectStatusID; // Use the manual status
            }

            // Rule 2: If the project has activities, calculate based on hierarchy.
            // The order of these checks is CRUCIAL as it implements the hierarchy.
            // We look for the "highest priority" (most critical/blocking) status first.

            // 1. Check for Incomplete
            if (project.Activities.Any(a => a.ActivityProgress == (int)ProjectActivityStatus.Incomplete))
            {
                return (int)ProjectActivityStatus.Incomplete;
            }

            // 2. Check for On-Hold
            if (project.Activities.Any(a => a.ActivityProgress == (int)ProjectActivityStatus.OnHold))
            {
                return (int)ProjectActivityStatus.OnHold;
            }

            // 3. Check if ALL activities are Completed
            // This means no other (non-completed) activity status exists.
            // This check must come *before* general "Ongoing" or "Started" checks.
            if (project.Activities.All(a => a.ActivityProgress == (int)ProjectActivityStatus.Completed))
            {
                return (int)ProjectActivityStatus.Completed;
            }

            // 4. Check for Ongoing
            if (project.Activities.Any(a => a.ActivityProgress == (int)ProjectActivityStatus.Ongoing))
            {
                return (int)ProjectActivityStatus.Ongoing;
            }

            // 5. Check for Sent for Review
            if (project.Activities.Any(a => a.ActivityProgress == (int)ProjectActivityStatus.SentForReview))
            {
                return (int)ProjectActivityStatus.SentForReview;
            }

            // 6. Check for Started
            if (project.Activities.Any(a => a.ActivityProgress == (int)ProjectActivityStatus.Started))
            {
                return (int)ProjectActivityStatus.Started;
            }

            // 7. Fallback: If no other conditions met, and activities exist, they must all be Upcoming.
            // This could also be a check: if (project.Activities.All(a => a.ActivityProgress == (int)ProjectActivityStatus.Upcoming))
            return (int)ProjectActivityStatus.Upcoming;
        }
    }
}