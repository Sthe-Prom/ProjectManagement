using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProjectManagement.Models;

namespace ProjectManagement.Models
{
    public class AccountViewModel: BaseViewModel
    {
        public int AccountID { get; set; }

        [Required(ErrorMessage = "Please enter your First Name:")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your Surname:")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter your email:")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your Cell:")]
        public string Phone { get; set; }      

        [Required(ErrorMessage = "Please Log in or Register Your account")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Please select designation")]
        public int DesignationId { get; set; }

        [Required(ErrorMessage = "Please select subunit")]
        public int UnitId { get; set; }

        public Microsoft.AspNetCore.Mvc.Rendering.SelectList Users { get; set; }
        public Microsoft.AspNetCore.Mvc.Rendering.SelectList Subdepts { get; set; }
        public Microsoft.AspNetCore.Mvc.Rendering.SelectList Designation { get; set; }
    }
}