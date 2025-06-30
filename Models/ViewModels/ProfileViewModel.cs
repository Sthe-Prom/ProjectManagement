using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProjectManagement.Models;

namespace ProjectManagement.Models
{
    public class ProfileViewModel: BaseViewModel
    {
        public Account Account { get; set; }
        public AccountViewModel ProfileModel { get; set; }
        public Microsoft.AspNetCore.Mvc.Rendering.SelectList Subdepts { get; set; }
        public Microsoft.AspNetCore.Mvc.Rendering.SelectList Users { get; set; }
        public Microsoft.AspNetCore.Mvc.Rendering.SelectList Designation { get; set; }

        public IEnumerable<Subdept> SubdeptsList { get; set; }
        public IEnumerable<Subdept> DesignationList { get; set; }
    }
}