using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagement.Models;

namespace ProjectManagement.Interfaces
{
    public interface IAccount
    {
        IEnumerable<Account> Accounts { get; }

        Task SaveAccount(Account Account);

        Account DeleteAccount(string AccountID);
    }
}