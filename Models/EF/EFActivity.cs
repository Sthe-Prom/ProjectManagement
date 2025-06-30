using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagement.Interfaces;
using ProjectManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Models
{
    public class EFActivity: IActivity
    {
        public AppDbContext context;
        
        public EFActivity(AppDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<Activity> Activities => context.Activity;

        public async Task<IEnumerable<Activity>> GetAllActivities()
        {
            return await context.Activity.ToListAsync();
        }

        public async Task<Activity> GetActivityById(int id)
        {
            return await context.Activity.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task SaveActivity(Activity Activity)
        {
            if (Activity.Id == 0)
            {
                context.Activity.AddAsync(Activity);

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
                context.Update(Activity);

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

        public Activity DeleteActivity(int ActivityID)
        {
            Activity dbEntry = context.Activity
                .FirstOrDefault(c => c.Id == ActivityID);

            if (dbEntry != null)
            {
                context.Activity.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }
    }
}