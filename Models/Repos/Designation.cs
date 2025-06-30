using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagement.Models
{
    public class Designation
    {
        public int Id { get; set; }        // This will be used for the <option value="">
        public string Name { get; set; } // This will be used for the <option>Text</option>
    
        // Optional constructor for easy creation
        public Designation(int Id_, string Name_)
        {
            Id = Id_;
            Name = Name_;
        }

    }
}