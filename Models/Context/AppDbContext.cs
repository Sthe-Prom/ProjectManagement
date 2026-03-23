using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Models;

namespace ProjectManagement.Models
{
    public class AppDbContext: DbContext
    {
         public AppDbContext(DbContextOptions<AppDbContext> options)
         : base(options) { }

        //Propeties - [T A B L E S]
        public DbSet<Project> Project { get; set; }
        public DbSet<Activity> Activity { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Subdept> Subdept { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<ProjectType> ProjectType { get; set; }
        public DbSet<KPI> KPI { get; set; }
       
    }
}