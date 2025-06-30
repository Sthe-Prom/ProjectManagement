using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagement.Models;

namespace ProjectManagement.Interfaces
{
    public interface IActivity
    {
        IEnumerable<Activity> Activities { get; }

        Task SaveActivity(Activity Activity);

        Activity DeleteActivity(int ActivityID);
    }
}