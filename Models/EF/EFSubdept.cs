using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagement.Interfaces;
using ProjectManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Models
{
    public class EFSubdept: ISubdept
    {
        public AppDbContext context;      

        public EFSubdept(AppDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<Subdept> Subdepts => context.Subdept;

        public async Task<IEnumerable<Subdept>> GetAllSubdeptes()
        {
            return await context.Subdept.ToListAsync();
        }

        public async Task<Subdept> GetSubdeptById(int id)
        {
            return await context.Subdept.FirstOrDefaultAsync(c => c.UnitId == id);
        }

        public async Task SaveSubdept(Subdept Subdept)
        {
            if (Subdept.UnitId == 0)
            {
                await context.Subdept.AddAsync(Subdept);

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
                context.Update(Subdept);

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

        public Subdept DeleteSubdept(int SubdeptID)
        {
            Subdept dbEntry = context.Subdept
                .FirstOrDefault(c => c.UnitId == SubdeptID);

            if (dbEntry != null)
            {
                context.Subdept.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }
    }
}