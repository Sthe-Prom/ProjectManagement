using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagement.Models;

namespace ProjectManagement.Interfaces
{
    public interface ISubdept
    {
        IEnumerable<Subdept> Subdepts { get; }

        Task SaveSubdept(Subdept Subdept);

        Subdept DeleteSubdept(int SubdeptID);        
    }
}