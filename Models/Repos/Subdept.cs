using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagement.Models
{
    public class Subdept
    {
         [Key]
        public int UnitId {get;set;}
        public string SubdeptName {get; set; }
    }
}