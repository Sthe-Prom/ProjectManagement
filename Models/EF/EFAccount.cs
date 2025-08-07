using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagement.Interfaces;
using ProjectManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Models
{
    public class EFAccount: IAccount
    {
        public AppDbContext context;
               
        public IEnumerable<Account> Accounts => context.Account;

        public EFAccount(AppDbContext ctx)
        {
            context = ctx;
        }

        public async Task<IEnumerable<Account>> GetAllAccounts()
        {
            return await context.Account.ToListAsync();
        }

        public async Task<Account> GetAccountById(int id)
        {
            return await context.Account.FirstOrDefaultAsync(c => c.AccountID == id);
        }

        public async Task SaveAccount(Account Account)
        {
            if (Account.AccountID == 0)
            {
                await context.Account.AddAsync(Account);

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
                context.Update(Account);

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

        public Account DeleteAccount(int AccountID)
        {
            Account dbEntry = context.Account
                .FirstOrDefault(c => c.AccountID == AccountID);

            if (dbEntry != null)
            {
                context.Account.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }
    }
}