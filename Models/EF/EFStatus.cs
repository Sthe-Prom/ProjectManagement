using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagement.Interfaces;
using ProjectManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Models
{
    public class EFStatus: IStatus
    {
        public AppDbContext context;

        public IEnumerable<Status> Statuses => context.Status;

        public EFStatus(AppDbContext ctx)
        {
            context = ctx;
        }

        public async Task<IEnumerable<Status>> GetAllStatuses()
        {
            return await context.Status.ToListAsync();
        }

        public async Task<Status> GetStatusById(int id)
        {
            return await context.Status.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task SaveStatus(Status Status)
        {
            if (Status.Id == 0)
            {
                await context.Status.AddAsync(Status);

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
                context.Update(Status);

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

        public Status DeleteStatus(int StatusID)
        {
            Status dbEntry = context.Status
                .FirstOrDefault(c => c.Id == StatusID);

            if (dbEntry != null)
            {
                context.Status.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }
    }
}