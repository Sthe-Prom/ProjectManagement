using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagement.Models
{
    public class KPI
    {
        [Key]
        public int Id {get;set;}
        public string KPIName {get; set; }
    }
}