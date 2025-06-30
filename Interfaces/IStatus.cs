using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagement.Models;

namespace ProjectManagement.Interfaces
{
    public interface IStatus
    {
        IEnumerable<Status> Statuses { get; }

        Task SaveStatus(Status Status);

        Status DeleteStatus(int StatusID);
    }
}