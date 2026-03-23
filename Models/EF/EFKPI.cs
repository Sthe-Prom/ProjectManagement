using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagement.Interfaces;
using ProjectManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Models
{
    public class EFKPI: IKPI
    {
        public AppDbContext context;

        public IEnumerable<KPI> KPIs => context.KPI;

        public EFKPI(AppDbContext ctx)
        {
            context = ctx;
        }
    }
}