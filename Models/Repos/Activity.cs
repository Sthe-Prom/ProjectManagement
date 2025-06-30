using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagement.Models
{
    public class Activity
    {
        [Key]
        public int Id {get;set;}
        public string ActivityName {get; set; }
        public string ActivityComments {get; set; }
        public string ActivityChallenges {get; set; }
        public string ActivityHighlights {get; set; }
        public DateTime ActivityStartDate {get; set; } = DateTime.UtcNow;
        public DateTime ActivityEndDate {get; set; } = DateTime.UtcNow;
        public DateTime ActivityUpdateTime {get; set; } = DateTime.UtcNow;
        public string ProjectFiles {get; set; }
        public int ActivityProgress {get; set; }
 
        /* Relationship
         FKs         
        */
        [Required(ErrorMessage = "No Project id")]
        public int ProjectID { get; set; }

        /* Ref Nav Properties */
        [ForeignKey("ProjectID")]
        public virtual Project Project { get; set; }
    }
}