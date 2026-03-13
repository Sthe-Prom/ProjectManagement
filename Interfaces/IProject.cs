using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagement.Models;

namespace ProjectManagement.Interfaces
{
    public interface IProject
    {
        IEnumerable<Project> Projects { get; }

        Task<Project?> GetProjectById(int Id);

        Task SaveProject(Project Project);

        Project DeleteProject(int ProjectID);
    }
}