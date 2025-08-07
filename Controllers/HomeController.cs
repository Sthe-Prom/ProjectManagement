using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Models;
using ProjectManagement.Services;
using ProjectManagement.Interfaces;
using Newtonsoft.Json;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using QuestPDF.Drawing;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Controllers;

public class HomeController : Controller
{    
    private readonly IAccount account_context;
    private readonly ISubdept subdept_context;
    private readonly IProject project_context;
    private readonly IActivity activity_context;
    private readonly IStatus status_context;
    private readonly IWebHostEnvironment HostEnvironment;
    public BaseViewModel BaseViewModel { get; set; }
    private readonly ILogger<HomeController> _logger;

    //Services
    private readonly IProjectAction project_service;

    //User Login
    private UserManager<User> UserManager;
    private SignInManager<User> signInManager;

     // This list would typically come from a database query, service, etc.
    private List<ActivityStatus> ActivityStatusList = new List<ActivityStatus>
    {
        new ActivityStatus(1, "Upcoming"),
        new ActivityStatus(2, "Started"),
        new ActivityStatus(3, "Ongoing"),
        new ActivityStatus(4, "Completed"),
        new ActivityStatus(5, "Incomplete"),
        new ActivityStatus(6, "On-Hold"),
        new ActivityStatus(7, "Sent for Review")       
    };

    // Public property to hold the SelectList
    public SelectList ActivityStatusSelectList { get; set; }

    public HomeController(ILogger<HomeController> logger, IAccount account_context_, ISubdept subdept_context_,
                          IProject project_context_, IActivity activity_context_, IStatus status_context_,
                          UserManager<User> userManager_, SignInManager<User> signInManager_, IWebHostEnvironment he,
                          IProjectAction project_service_)
    {
        _logger = logger;

        //Project Objects
        project_context = project_context_;
        activity_context = activity_context_;
        subdept_context = subdept_context_;
        status_context = status_context_;
        
        //User Management
        UserManager = userManager_;
        signInManager = signInManager_;
        account_context = account_context_;

        //Context
        HostEnvironment = he;

        //Services
        project_service = project_service_;

        this.BaseViewModel = new BaseViewModel();
        this.BaseViewModel.Accounts = account_context.Accounts;
        this.ViewData["BaseViewModel"] = this.BaseViewModel;
    }

    public async Task<IActionResult> Index(ProjectViewModel vm, int? Status, int? Staff)
    {
        //IEnumerable<Project> projects = project_context.Projects;
        //var projects_ = projects;
      
        vm.ProjectModel = new Project();
        vm.fileModel = new FileViewModel();
        vm.ActivityModel = new ProjectManagement.Models.Activity();
        vm.Projects = project_context.Projects;       
        vm.Activities = activity_context.Activities;
        vm.Accounts = account_context.Accounts;
        vm.Statuses = status_context.Statuses;
        vm.UserAccounts = GetAccountsAsync();
        vm.StatusList = getStatuses();
        vm.ActivityStatuses = getActivityStatus();
        
        var Project_ = project_context.Projects;
        
        //Filter By User 
        if (!string.IsNullOrEmpty(Request.Query["Staff"]))
        {            
            vm.Projects = project_context.Projects
                    .Where(a => a.AccountID == Staff);   
            
            var p = project_context.Projects
                    .Where(a => a.AccountID == Staff)
                    .Select(a => a.Id);

            vm.Activities = activity_context.Activities
                    .Where(a => p.Contains(a.ProjectID));

            Project_ = project_context.Projects
                    .Where(a => a.AccountID == Staff) ;       
            
        }            

        //Filter by Stasues 
        if (!string.IsNullOrEmpty(Request.Query["Status"]))
        {            
            vm.Projects = project_context.Projects
                    .Where(p => p.ProjectStatusID == Status);   
            
            var p = project_context.Projects
                    .Where(p => p.ProjectStatusID == Status)
                    .Select(p => p.Id);

            vm.Activities = activity_context.Activities
                    .Where(a => p.Contains(a.ProjectID));

            Project_ = project_context.Projects
                    .Where(a => a.ProjectStatusID == Status) ;       
            
        }                     

        List<DataPoint> dataPoints = new List<DataPoint>();
        List<DataPoint> dataPointsActivity = new List<DataPoint>();
        List<DataPoint> dataPointsCompleted = new List<DataPoint>();
        List<DataPoint> dataPointsIncomplete = new List<DataPoint>();
        List<DataPoint> dataPointsStarted = new List<DataPoint>();
        List<DataPoint> dataPointsOther = new List<DataPoint>();

        //Apex
        List<int> dataPointsCompleted_apex = new List<int>();
        List<int> dataPointsIncomplete_apex = new List<int>();
        List<int> dataPointsStarted_apex = new List<int>();
        List<string> dataPointsStarted_apexCategory = new List<string>();

        //Echarts
        List<DataPointE> dataPoints_eCharts = new List<DataPointE>();
        List<string> projectCategories = new List<string>();
        List<int> projectValues = new List<int>();

        var projects_data = from acc in account_context.Accounts
                    join proj in Project_
                    on acc.AccountID equals proj.AccountID
                    join stat in status_context.Statuses
                    on proj.ProjectStatusID equals stat.Id
                    group new {proj, stat}     
                        by new {Year = proj.ProjectStartDate.Year, Month = proj.ProjectStartDate.Month, stat.StatusName} 
                    into g     
                  select new {
                    Status = g.Key.StatusName,
                    ProjectDate = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month), 
                    SortDate = new DateTime(g.Key.Year, g.Key.Month, 1),  
                    ProjectCount = g.Count()                             

                  };

        var projects_sortedMonthly = projects_data.OrderBy(m => m.SortDate).ToList();                  

        var activities_data = from acc in account_context.Accounts
                    join proj in Project_
                    on acc.AccountID equals proj.AccountID             
                    join act in activity_context.Activities
                    on proj.Id equals act.ProjectID
                    join stat in status_context.Statuses
                    on act.ActivityProgress equals stat.Id
                    group new {stat, act}     
                        by new {stat.StatusName, act.ActivityProgress} 
                    into g     
                  select new {
                    Status = g.Key.StatusName,
                    ActivityProgress = g.Key.ActivityProgress,
                    ActivityCount = g.Count()              

                  };

        foreach(var item in projects_data)
        {
            //Main Projects
            dataPoints.Add(new DataPoint(item.Status, item.ProjectCount) ); 
            var c = item.ProjectCount;

            //Projects Categories (Status)
            if(!projectCategories.Contains(item.Status))
            {
                projectCategories.Add(item.Status);
                projectValues.Add(c);
            } 
            else
            {
                c = c+c;
                projectValues.Add(c);
            }
        }

        foreach(var item in projects_sortedMonthly)
        {                               
            //Apex Categories (Months)
            if(!dataPointsStarted_apexCategory.Contains(item.ProjectDate))
            {
                dataPointsStarted_apexCategory.Add(item.ProjectDate);
            }

            //Line Charts
            switch (item.Status)
            {
                case "Completed":
                    dataPointsCompleted.Add(new DataPoint(item.ProjectDate, item.ProjectCount));
                    dataPointsCompleted_apex.Add(item.ProjectCount);          
                    break;
                case "Incomplete":
                    dataPointsIncomplete.Add(new DataPoint(item.ProjectDate, item.ProjectCount));
                    dataPointsIncomplete_apex.Add(item.ProjectCount);
                    break;
                case "Started":
                    dataPointsStarted.Add(new DataPoint(item.ProjectDate, item.ProjectCount));
                    dataPointsStarted_apex.Add(item.ProjectCount);                   
                    break;
                default:
                    dataPointsOther.Add(new DataPoint(item.ProjectDate, item.ProjectCount));
                    break;
            }    

        }
       
        foreach(var item in activities_data)
        {
            dataPointsActivity.Add(new DataPoint(item.Status, item.ActivityCount) );
        }

        //Prepare Json Output
        ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);    
        ViewBag.DataPointsActivity = JsonConvert.SerializeObject(dataPointsActivity);    
        ViewBag.DataPointsComplete = JsonConvert.SerializeObject(dataPointsCompleted); 
        ViewBag.DataPointsIncomplete = JsonConvert.SerializeObject(dataPointsIncomplete); 
        ViewBag.DataPointsStarted = JsonConvert.SerializeObject(dataPointsStarted); 
        ViewBag.DataPointsOther = JsonConvert.SerializeObject(dataPointsOther);  

        //Apex charts
        ViewBag.DataApexComplete_Apex = JsonConvert.SerializeObject(dataPointsCompleted_apex); 
        ViewBag.DataPointsIncomplete_Apex = JsonConvert.SerializeObject(dataPointsIncomplete_apex); 
        ViewBag.DataPointsStarted_Apex = JsonConvert.SerializeObject(dataPointsStarted_apex); 
        ViewBag.DataPointsStarted_apexCategory = JsonConvert.SerializeObject(dataPointsStarted_apexCategory);       

        //ECharts
        ViewBag.DataPoints_eCharts_Projects = JsonConvert.SerializeObject(dataPoints_eCharts);       
        ViewBag.ProjectCategories = JsonConvert.SerializeObject(projectCategories);    
        ViewBag.ProjectValues = JsonConvert.SerializeObject(projectValues);      

        foreach(var project in Project_)
        {
            project.CalculatedDisplayStatusId = await project_service.GetProjectDisplayStatus(project);
            project.CalculatedDisplayStatusName = GetStatusNameById(project.CalculatedDisplayStatusId);
        } 

        return View(vm);
    }

    // Helper method to get status name (can also be in ProjectService or a separate utility)
    private string GetStatusNameById(int statusId)
    {
        // Use your ProjectActivityStatus enum or a lookup table/dictionary
        switch (statusId)
        {
            case (int)ProjectActivityStatus.Upcoming: return "Upcoming";
            case (int)ProjectActivityStatus.Started: return "Started";
            case (int)ProjectActivityStatus.Ongoing: return "Ongoing";
            case (int)ProjectActivityStatus.Completed: return "Completed";
            case (int)ProjectActivityStatus.Incomplete: return "Incomplete";
            case (int)ProjectActivityStatus.OnHold: return "On-Hold";
            case (int)ProjectActivityStatus.SentForReview: return "Sent for Review";
            default: return "Unknown";
        }
    }

    [HttpPost]
    public async Task<JsonResult> AddProjectAjax(ProjectViewModel vm, IFormCollection formCollection)
    { 
        //Prepare Json Response
        JsonViewModel model = new JsonViewModel();
        //string uploadedActionFile = UploadedFile(vm);
        string uniqueFileName = "";

        if (vm.ProjectFiles == null || vm.ProjectFiles.Length == 0)
        {
            //model.ResponseMessage = "No file uploded.";
            uniqueFileName = "na";
        }
        else
        {
            //vm.fileModel = new FileViewModel();
            //vm_.fileModel.ProjectFiles = formCollection["ProjectModel_ProjectName"].ToString();

            // Get file information
            //var fileName = Path.GetFileName(file.FileName);
            //var fileSize = file.Length;
            //var contentType = file.ContentType;
            uniqueFileName = Guid.NewGuid().ToString().Substring(0, 3) + "_" + vm.ProjectFiles.FileName;
        

            string uploadsFolder = Path.Combine(HostEnvironment.WebRootPath, "Attachments/Projects");
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                vm.ProjectFiles.CopyTo(fileStream);
            } 
        }
       
        List<int> selectedUserIds = new List<int>();
        string selectedIdsString = formCollection["ProjectModel_SelectedAssignedUserIds"].ToString();
        
        //Add Current User
        var currentUser = await UserManager.GetUserAsync(User);
        var project_owner = account_context.Accounts.Where(c => c.Id == currentUser.Id).FirstOrDefault();
        selectedUserIds.Add(project_owner.AccountID);

        if (!string.IsNullOrEmpty(selectedIdsString))
        {
            // 2. Split the string by comma
            if(selectedIdsString.Contains(','))
            {
                string[] idStrings = selectedIdsString.Split(',');

                 // 3. Parse each string part into an integer and add to the list
                foreach (string idStr in idStrings)
                {
                    if (int.TryParse(idStr.Trim(), out int userId))
                    {
                        selectedUserIds.Add(userId);
                    }
                    else
                    {
                        // Handle parsing error if necessary (e.g., log it, return error)
                        model.ResponseMessage = $"Warning: Could not parse '{idStr}' into an integer.";
                    }
                }
            }
            else
            {
                selectedUserIds.Add(Convert.ToInt32(selectedIdsString));
            }

           
        }
        else
        {
            // No users were selected or the input was empty
            model.ResponseMessage = "No user IDs selected.";
        }

        var Project = new Project()
        {            
            ProjectName = formCollection["ProjectModel_ProjectName"].ToString(),
            ProjectDetails = formCollection["ProjectModel_ProjectDetails"].ToString(),
            ProjectAreaOfWork = formCollection["ProjectModel_ProjectAreaOfWork"].ToString(),
            ProjectWorkInConj = formCollection["ProjectModel_ProjectWorkInConj"].ToString(),          
            ProjectFiles = uniqueFileName,
            ProjectUpdateTime = DateTime.UtcNow,
            ProjectStartDate =  Convert.ToDateTime(formCollection["ProjectModel_ProjectStartDate"]),
            ProjectEndDate =  Convert.ToDateTime(formCollection["ProjectModel_ProjectEndDate"]),
            ProjectStatusID = Convert.ToInt32(formCollection["ProjectModel_ProjectStatusID"]),
            AccountID = Convert.ToInt32(formCollection["ProjectModel_AccountID"]),
            SelectedAssignedUserIds = selectedUserIds
            
        };

       
        try
        {
            await project_context.SaveProject(Project);
        }
        catch (Exception ex)
        {
            model.ResponseCode = 1;
        }

        if (Project != null)
        {
            model.ResponseCode = 0;
            if(Project.ProjectName.Length >= 20)
            {
                model.ResponseMessage = JsonConvert.SerializeObject(Project.ProjectName.Substring(0, 15) + "... "); //"Record Added";
            }else{
                model.ResponseMessage = JsonConvert.SerializeObject(Project.ProjectName);
            }
           
        }
        else
        {
            model.ResponseCode = 1;
            //model.ResponseMessage = "No record available";
        }

        return Json(model);

    }    

    [HttpPost]
    public async Task<JsonResult> AddActivityAjax(ProjectViewModel vm, IFormCollection formCollection)
    { 
        //Prepare Json Response
        JsonViewModel model = new JsonViewModel();

        //string uploadedActionFile = UploadedFile(vm);
        string uniqueFileName = "";

        if (vm.ProjectFiles == null || vm.ProjectFiles.Length == 0)
        {
            //model.ResponseMessage = "No file uploded.";
            uniqueFileName = "na";
        }
        else
        {
            //vm.fileModel = new FileViewModel();
            //vm_.fileModel.ProjectFiles = formCollection["ProjectModel_ProjectName"].ToString();

            // Get file information
            //var fileName = Path.GetFileName(file.FileName);
            //var fileSize = file.Length;
            //var contentType = file.ContentType;
            uniqueFileName = Guid.NewGuid().ToString().Substring(0, 3) + "_" + vm.ProjectFiles.FileName;
       

            string uploadsFolder = Path.Combine(HostEnvironment.WebRootPath, "Attachments/Activity");
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                vm.ProjectFiles.CopyTo(fileStream);
            }        
        }

        var memberProject = 0;
        if(!String.IsNullOrEmpty(formCollection["ActivityModel_MemberProject"]))
        {
            memberProject = Convert.ToInt32(formCollection["ActivityModel_MemberProject"]);
        }
        else
        {
            memberProject = 0;
        }
        
        var Activity = new ProjectManagement.Models.Activity()
        {            
            ActivityName = formCollection["ActivityModel_ActivityName"].ToString(),
            ActivityComments = formCollection["ActivityModel_ActivityComments"].ToString(),
            ActivityChallenges = formCollection["ActivityModel_ActivityChallenges"].ToString(),
            ActivityHighlights = formCollection["ActivityModel_ActivityHighlights"].ToString(),          
            ProjectFiles = uniqueFileName,
            ActivityUpdateTime = DateTime.UtcNow,
            ActivityStartDate =  Convert.ToDateTime(formCollection["ActivityModel_ActivityStartDate"]),
            ActivityEndDate =  Convert.ToDateTime(formCollection["ActivityModel_ActivityEndDate"]),
            ActivityProgress = Convert.ToInt32(formCollection["ActivityModel_ActivityProgress"]),
            ProjectID = Convert.ToInt32(formCollection["ActivityModel_ProjectID"]),
            MemberProject = memberProject,          
            
        };
              
        try
        {
            await activity_context.SaveActivity(Activity);      
            
        }
        catch (Exception ex)
        {
            model.ResponseCode = 1;
        }

        if (Activity != null)
        {
            model.ResponseCode = 0;
            if(Activity.ActivityName.Length >= 20)
            {
                model.ResponseMessage = JsonConvert.SerializeObject(Activity.ActivityName.Substring(0, 15) + "... "); //"Record Added";
            }else{
                model.ResponseMessage = JsonConvert.SerializeObject(Activity.ActivityName);
            }
           
        }
        else
        {
            model.ResponseCode = 1;
            model.ResponseMessage = "No record available";
        }

        return Json(model);

    }

    [HttpPost]
    public async Task<JsonResult> UpdateProject(IFormCollection formCollection, ProjectViewModel vm, int Id)
    {
        //var proj_Id = Convert.ToInt32(formCollection["projectID"]);
        //var Project = project_context.Projects.FirstOrDefault(c => c.Id == 26);
        var Project = project_context.Projects.Where(c => c.Id == Id).FirstOrDefault();  

        //Prepare Json Response
        JsonViewModel model = new JsonViewModel();
        // //string uploadedActionFile = UploadedFile(vm);
        string uniqueFileName = "";

        if (vm.ProjectFiles == null || vm.ProjectFiles.Length == 0)
        {
            //model.ResponseMessage = "No file uploded.";
            uniqueFileName = "na";
        }
        else
        {
            //vm.fileModel = new FileViewModel();
            //vm_.fileModel.ProjectFiles = formCollection["ProjectModel_ProjectName"].ToString();

            // Get file information
            //var fileName = Path.GetFileName(file.FileName);
            //var fileSize = file.Length;
            //var contentType = file.ContentType;
            uniqueFileName = Guid.NewGuid().ToString().Substring(0, 3) + "_" + vm.ProjectFiles.FileName;
            
            string uploadsFolder = Path.Combine(HostEnvironment.WebRootPath, "Attachments/Projects");
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                vm.ProjectFiles.CopyTo(fileStream);
            } 
        }
       
        List<int> selectedUserIds = Project.SelectedAssignedUserIds;//new List<int>();
        string selectedIdsString = formCollection["proj_SelectedAssignedUserIds"].ToString();

        if (!string.IsNullOrEmpty(selectedIdsString))
        {
            // 2. Split the string by comma
            if(selectedIdsString.Contains(','))
            {
                string[] idStrings = selectedIdsString.Split(',');

                 // 3. Parse each string part into an integer and add to the list
                foreach (string idStr in idStrings)
                {
                    if (int.TryParse(idStr.Trim(), out int userId))
                    {
                        if(!selectedUserIds.Contains(userId))
                        {
                            selectedUserIds.Add(userId);
                        }
                    }
                    else
                    {
                        // Handle parsing error if necessary (e.g., log it, return error)
                        model.ResponseMessage = $"Warning: Could not parse '{idStr}' into an integer.";
                    }
                }
            }
            else
            {
                selectedUserIds.Add(Convert.ToInt32(selectedIdsString));
            }

           
        }
        else
        {
            // No users were selected or the input was empty
            model.ResponseMessage = "No user IDs selected.";
        }    

        if (Project != default(Project))
        {                   
            Project.ProjectName = formCollection["proj_ProjectName"].ToString();
            Project.ProjectDetails = formCollection["proj_ProjectDetails"].ToString();
            Project.ProjectAreaOfWork = formCollection["proj_ProjectAreaOfWork"].ToString();
            Project.ProjectWorkInConj = formCollection["proj_ProjectWorkInConj"].ToString(); 

            
            if (vm.ProjectFiles == null || vm.ProjectFiles.Length == 0)
            {
                Project.ProjectFiles =  Project.ProjectFiles;    
            }
            else
            {
                Project.ProjectFiles = uniqueFileName;
            }   

            Project.ProjectUpdateTime = DateTime.UtcNow;
            Project.ProjectStartDate = Convert.ToDateTime(formCollection["proj_ProjectStartDate"]);
            Project.ProjectEndDate =  Convert.ToDateTime(formCollection["proj_ProjectEndDate"]);
            Project.ProjectStatusID = Convert.ToInt32(formCollection["proj_ProjectStatusID"]);
            Project.AccountID = Convert.ToInt32(formCollection["proj_AccountID"]);
            Project.SelectedAssignedUserIds = selectedUserIds;
        
            try
            {
                await project_context.SaveProject(Project);
            }
            catch (Exception ex)
            {
                model.ResponseCode = 0;
                model.ResponseMessage = "Issue is: " + ex.Message;
            }

            //Json Response
            if (Project != null)
            {
                model.ResponseCode = 0;
                if(Project.ProjectName.Length >= 15)
                {
                    model.ResponseMessage = JsonConvert.SerializeObject(Project.ProjectName.Substring(0, 14) + "... ");
                }else{
                    model.ResponseMessage = JsonConvert.SerializeObject(Project.ProjectName);
                }
                
            }
            else
            {
                model.ResponseCode = 1;
                model.ResponseMessage = "No record available";
            }

           
        }

        return Json(model);
    }

    [HttpPost]
    public async Task<JsonResult> UpdateActivity(IFormCollection formCollection, ProjectViewModel vm)
    {
        var act_Id = Convert.ToInt32(formCollection["ActivityModel_Id"]);
        var Activity = activity_context.Activities.FirstOrDefault(c => c.Id == act_Id);
      
        //Prepare Json Response
        JsonViewModel model = new JsonViewModel();
        string uniqueFileName = "";

        if (vm.ProjectFiles == null || vm.ProjectFiles.Length == 0)
        {
            model.ResponseMessage = "No file uploded.";
            uniqueFileName = "na";
        }
        else
        {          
            uniqueFileName = Guid.NewGuid().ToString().Substring(0, 3) + "_" + vm.ProjectFiles.FileName;        

            string uploadsFolder = Path.Combine(HostEnvironment.WebRootPath, "Attachments/Projects");
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                vm.ProjectFiles.CopyTo(fileStream);
            } 
        }

        if (Activity != default(ProjectManagement.Models.Activity))
        {                         
            Activity.ActivityName = formCollection["act_ActivityName"].ToString();
            Activity.ActivityComments = formCollection["act_ActivityComments"].ToString();
            Activity.ActivityChallenges = formCollection["act_ActivityChallenges"].ToString();
            Activity.ActivityHighlights = formCollection["act_ActivityHighlights"].ToString();  

            if (vm.ProjectFiles == null || vm.ProjectFiles.Length == 0)
            {
                Activity.ProjectFiles =  Activity.ProjectFiles;    
            }
            else
            {
                Activity.ProjectFiles = uniqueFileName;
            }   

            Activity.ActivityUpdateTime = DateTime.UtcNow;
            Activity.ActivityStartDate =  Convert.ToDateTime(formCollection["act_ActivityStartDate"]);
            Activity.ActivityEndDate =  Convert.ToDateTime(formCollection["act_ActivityEndDate"]);
            Activity.ActivityProgress = Convert.ToInt32(formCollection["act_ActivityProgress"]); 
            //Activity.ProjectID = Convert.ToInt32(formCollection["ActivityModel_Id"]);
        
            try
            {
                await activity_context.SaveActivity(Activity);
            }
            catch (Exception ex)
            {
                model.ResponseCode = 0;
                model.ResponseMessage = "Issue is: " + ex.Message;
            }

            //Json Response
            if (Activity != null)
            {
                model.ResponseCode = 0;
                if(Activity.ActivityName.Length >= 15)
                {
                    model.ResponseMessage = JsonConvert.SerializeObject(Activity.ActivityName.Substring(0, 14) + "... ");
                }else{
                    model.ResponseMessage = JsonConvert.SerializeObject(Activity.ActivityName);
                }                
            }
            else
            {
                model.ResponseCode = 1;
                model.ResponseMessage = "No record available";
            }
           
        }

        return Json(model);
    }

    private string UploadedFile(ProjectViewModel model)
    {
        string uniqueFileName = "no file found";

        // if (model.fileModel.ProjectFiles != null)
        // {
        //     uniqueFileName = Guid.NewGuid().ToString().Substring(0, 3) + "_" + model.fileModel.ProjectFiles.FileName;
        // }

        return uniqueFileName;
    }


    public SelectList GetAccountsAsync()
    {
        // Get the ID of the current user
        // No need to await here, as GetUserId is not an async method
        var currentUserId = UserManager.GetUserId(User);

        // Get the IDs of all users who are in the "Admin" role
        // This is done once and asynchronously for better performance
        var adminUsers = UserManager.GetUsersInRoleAsync("Admin").Result;
        var adminUserIds = adminUsers.Select(u => u.Id).ToList();

        // Use a single LINQ query to filter out the current user and admin users
        // and select the necessary data for the SelectList
        var allOtherUsers = account_context.Accounts
                                            .Where(acc => acc.Id != currentUserId && !adminUserIds.Contains(acc.Id))
                                            .Select(acc => new
                                            {
                                                AccountId = acc.AccountID,
                                                FullName = acc.FirstName + " " + acc.LastName,
                                            }).ToList();
                                             // Execute the query and get the results

     
        // Create the SelectList directly from the filtered, projected data
        // The SelectList constructor is perfect for this scenario
        SelectList userSelect = new SelectList(allOtherUsers, "AccountId", "FullName");
        
        return userSelect;
    }

    public SelectList getStatuses()
    {
        List<Status> models = new List<Status>();

        var AllStatuses = from status in status_context.Statuses
                select new
                {
                    Id = status.Id,
                    StatusName = status.StatusName
                };

        foreach (var item in AllStatuses)
        {
            var m = new Status();
            
            m.Id = item.Id;
            m.StatusName = item.StatusName;
            models.Add(m);
        }
        
        
       SelectList userSelect = new SelectList(models, "Id", "StatusName");
       return userSelect;
    }

    [HttpDelete]
    public ActionResult DeleteProject(int Id)
    {        
        //Prepare Json Response
        JsonViewModel model = new JsonViewModel();
        
        //try
        //{
            
        // }
        // catch (Exception ex)
        // {
        //     model.ResponseCode = 1;
        //     model.ResponseMessage = "model problem";
        // }

        if (Id > 0)
        {
            
            var Project = project_context.Projects.Where(c => c.Id == Id).FirstOrDefault();

            model.ResponseCode = 0;
           // model.ResponseMessage = "Project Deleted successfully";
            if(Project.ProjectName.Length >= 20)
            {
                model.ResponseMessage = JsonConvert.SerializeObject("Project "+ Project.ProjectName.Substring(0, 15) + " was Deleted successfully.");
            }else{
                model.ResponseMessage = JsonConvert.SerializeObject("Project "+ Project.ProjectName + " was Deleted successfully.");
            }

            Project = project_context.DeleteProject(Id);
           
        }
        else
        {
            model.ResponseCode = 1;
            model.ResponseMessage = "Error deleting Project. No Id Found.";
        }

        return Json(model);
    }

    [HttpDelete]
    public ActionResult DeleteActivity(int Id)
    {        
        //Prepare Json Response
        JsonViewModel model = new JsonViewModel();
       
        if (Id > 0)
        {            
            var Activity = activity_context.Activities.Where(c => c.Id == Id).FirstOrDefault();

            model.ResponseCode = 0;
           // model.ResponseMessage = "Project Deleted successfully";
            if(Activity.ActivityName.Length >= 20)
            {
                model.ResponseMessage = JsonConvert.SerializeObject("Activity "+ Activity.ActivityName.Substring(0, 15) + " was Deleted successfully.");
            }else{
                model.ResponseMessage = JsonConvert.SerializeObject("Activity "+ Activity.ActivityName + " was Deleted successfully.");
            }

            Activity = activity_context.DeleteActivity(Id);
           
        }
        else
        {
            model.ResponseCode = 1;
            model.ResponseMessage = "Error deleting Project. No Id Found.";
        }

        return Json(model);
    }

    public SelectList getActivityStatus()
    {       
        return ActivityStatusSelectList = new SelectList(ActivityStatusList, "Id", "Name");
    }

    public async Task<Project> GetProjectForDisplay(int projectId)
    {
        var project = project_context.Projects                                       
                .FirstOrDefault(c => c.Id == projectId);       
        
        if (project == null)
        {                 
            return null;
        }

        int calculatedDisplayStatusId = await project_service.GetProjectDisplayStatus(project);
        
        return project;
    }

}
