using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProjectManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjectManagement.Models
{
    public class ProjectViewModel: BaseViewModel
    {
        //Models
        public Project ProjectModel { get; set; }
        public IFormFile ProjectFiles {get; set; }
        public FileViewModel fileModel { get; set; }
        public Activity ActivityModel { get; set; }
        public Status StatusModel { get; set; }
        public Subdept SubdeptModel { get; set; } 
        public ProjectType ProjectType { get; set; }        

        //Models in Lists
        public IEnumerable<Project> Projects { get; set; }
        public IEnumerable<Activity> Activities { get; set; }
        public IEnumerable<Status> Statuses { get; set; }
        public IEnumerable<Subdept> Subdepts { get; set; }
        public IEnumerable<ProjectType> ProjectTypes { get; set; }
        public List<int> SelectedAssignedUserIds { get; set; }
        public SelectList UserAccounts { get; set; }
        public SelectList StatusList { get; set; }        
        public SelectList ActivityStatuses { get; set; }
        public SelectList ProjectTypeList { get; set; }
        
       
    }
}