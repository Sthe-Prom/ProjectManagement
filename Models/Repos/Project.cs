using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagement.Models
{
    public class Project
    {
        [Key]
        public int Id { get;set; }
        public string ProjectName { get; set; }
        public string ProjectDetails { get; set; }
        public DateTime ProjectStartDate { get; set; } = DateTime.UtcNow;
        public DateTime ProjectEndDate { get; set; } = DateTime.UtcNow;
        public DateTime ProjectUpdateTime { get; set; } = DateTime.UtcNow;
        public string? ProjectWorkInConj { get; set; }
        public string ProjectAreaOfWork { get; set; }       
        public string ProjectFiles { get; set; }
        //public List<Account> AssignedUsers { get; set; }
        public List<int> SelectedAssignedUserIds { get; set; } 
 
        /* Relationship
         FKs         
        */
        [Required(ErrorMessage = "No Project Status ID")]
        public int ProjectStatusID { get; set; }

        [Required(ErrorMessage = "No Project Status ID")]
        public int AccountID { get; set; }
        
        public int? ProjectTypeID { get; set; }

        public int? ProjectKPI { get; set; }

        /* Ref Nav Properties */
        [ForeignKey("ProjectStatusID")]
        public virtual Status Status { get; set; }

        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }

        // [ForeignKey("ProjectTypeID")]
        public virtual ProjectType ProjectType { get; set; }

        public ICollection<Activity> Activities {get; set;} = new List<Activity>();

        // NEW: Property to hold the calculated display status, not mapped to the database
        [NotMapped]
        public int CalculatedDisplayStatusId { get; set; }

        // NEW: Optional - A string version of the calculated status for easier display
        [NotMapped]
        public string CalculatedDisplayStatusName { get; set; }

    }
}