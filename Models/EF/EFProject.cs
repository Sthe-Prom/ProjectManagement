using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagement.Interfaces;
using ProjectManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Models
{
    public class EFProject: IProject
    {
        public AppDbContext context;

        public IEnumerable<Project> Projects => context.Project;

        public EFProject(AppDbContext ctx)
        {
            context = ctx;
        }

        public async Task<IEnumerable<Project>> GetAllProjects()
        {
            return await context.Project.ToListAsync();
        }

        public async Task<Project> GetProjectById(int id)
        {
            return await context.Project.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task SaveProject(Project Project)
        {
            if (Project.Id == 0)
            {
                context.Project.AddAsync(Project);

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {                    
                    throw new Exception("Failed to add.", ex);
                }
            }
            else
            {
                context.Update(Project);

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {                
                    // Handle exceptions (e.g., log error, throw specific exception)    
                    throw new Exception("Failed to update url.", ex);
                }
            }
          

        }

        public Project DeleteProject(int ProjectID)
        {
            Project dbEntry = context.Project
                .FirstOrDefault(c => c.Id == ProjectID);

            if (dbEntry != null)
            {
                context.Project.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }
    }
}