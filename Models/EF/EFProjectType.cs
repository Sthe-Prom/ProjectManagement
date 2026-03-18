using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagement.Interfaces;
using ProjectManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Models
{
    public class EFProjectType: IProjectType
    {
        public AppDbContext context;

        public IEnumerable<ProjectType> ProjectTypes => context.ProjectType;

        public EFProjectType(AppDbContext ctx)
        {
            context = ctx;
        }
    }
}