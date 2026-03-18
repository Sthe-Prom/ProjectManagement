using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagement.Models;

namespace ProjectManagement.Interfaces
{
    public interface IProjectType
    {
        IEnumerable<ProjectType> ProjectTypes { get; }
    }
}