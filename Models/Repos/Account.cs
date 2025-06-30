using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagement.Models
{
    public class Account
    {
        [Key]
        public int AccountID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int DesignationId { get; set; }

        /* Relationship
           FKs         
        */

        [Required(ErrorMessage = "Please enter user id")]
        //[StringLength(50)]
        public string Id { get; set; }

        [Required(ErrorMessage = "No Unit ID")]
        public int UnitId { get; set; }

        /* Ref Nav Properties */
        [ForeignKey("Id")]
        public virtual User User { get; set; }

        [ForeignKey("UnitId")]
        public virtual Subdept Subdept { get; set; }

    }
}