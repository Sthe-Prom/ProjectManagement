using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ProjectManagement.Models;
using ProjectManagement.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace ProjectManagement.Controllers;

public class AccountController : Controller
{ 
    //Private Fields
    //--------------

    private IAccount context;
    private readonly IWebHostEnvironment HostEnvironment;
    private readonly ISubdept subdept_context;
    private IConfiguration Configuration;
    public BaseViewModel BaseViewModel { get; set; }

    private SignInManager<User> signInManager;
    private UserManager<User> userManager;


    // This list would typically come from a database query, service, etc.
    private List<Designation> DesignationList = new List<Designation>
    {
        new Designation(1, "Institutional Researcher"),
        new Designation(2, "Data Analyst"),
        new Designation(3, "Strategy and Planning"),
        new Designation(4, "Digitalisation")
    };

    
    // Public property to hold the SelectList
    public SelectList DesignationSelectList { get; set; }

    //Constructor
    //-----------
    public AccountController(IAccount ctx, SignInManager<User> s_man, IWebHostEnvironment he,
                        UserManager<User> u_man, IConfiguration configuration, ISubdept subdept_context_)
    {
        context = ctx;
        Configuration = configuration;
        signInManager = s_man;
        userManager = u_man;
        HostEnvironment = he;
        subdept_context = subdept_context_;

        this.BaseViewModel = new BaseViewModel();
        this.BaseViewModel.Accounts = context.Accounts;
        this.ViewData["BaseViewModel"] = this.BaseViewModel;
    }

    [HttpGet]
    public ViewResult Index()
    {
        ProfileViewModel vm = new ProfileViewModel();
        vm.Accounts = context.Accounts;
        vm.Users = getUsers();
        vm.Subdepts = getSubdepts();
        vm.Designation = getDesignation();
        vm.SubdeptsList = subdept_context.Subdepts;
        vm.Account = new Account();
               
        vm.DesignationList = DesignationList;

        return View(vm);
    }

    [HttpGet]
    public IActionResult Profile()
    {
        ProfileViewModel vm = new ProfileViewModel()
        {
            Users = getUsers(),
            Subdepts = getSubdepts(),
            Designation = getDesignation(),
            Accounts = context.Accounts,
            Account = new Account()
        };

        return View(vm);
    }

    [HttpGet]
    public IActionResult Accounts()
    {
        ProfileViewModel vm = new ProfileViewModel()
        {
            Users = getUsers(),
            Subdepts = getSubdepts(),
            Designation = getDesignation(),
            Accounts = context.Accounts,
            Account = new Account()
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Profile(ProfileViewModel vm)
    {           
        string UserID_ = "";

        if (signInManager.IsSignedIn(User))
        {
            UserID_ = User.Identity.Name;
        }

        var Account = new Account
        {
            FirstName = vm.ProfileModel.FirstName,
            LastName = vm.ProfileModel.LastName,
            Phone = vm.ProfileModel.Phone,
            Email = vm.ProfileModel.Email,
            UnitId = vm.ProfileModel.UnitId,
            DesignationId = vm.ProfileModel.DesignationId,
            Id = vm.ProfileModel.Id 
        };

        // if (ModelState.IsValid)
        //{
            await context.SaveAccount(Account);
            return RedirectToAction("Index", "Account");
        // }
        // else
        // {
        //     return View("Profile");
        // }
    }

    // UPDATE
    [HttpGet]
    public ViewResult Manage(int AccountID)
    {
        ProfileViewModel vm = new ProfileViewModel();
        vm.Users = getUsers();   
        vm.Subdepts = getSubdepts();
        vm.Designation = getDesignation();
        vm.Accounts = context.Accounts;
        vm.Account = new Account();
        vm.ProfileModel = new AccountViewModel();

        Account Account = context.Accounts.FirstOrDefault(c => c.AccountID == AccountID);

        vm.ProfileModel.FirstName = Account.FirstName;
        vm.ProfileModel.LastName =  Account.LastName;
        vm.ProfileModel.Phone = Account.Phone;
        vm.ProfileModel.Email = Account.Email;
        vm.ProfileModel.UnitId = Account.UnitId;
        vm.ProfileModel.AccountID = Account.AccountID;
        vm.ProfileModel.DesignationId = Account.DesignationId;
        vm.ProfileModel.Id =  Account.Id;

        return View(vm);

    }

    [HttpPost]
    public async Task<IActionResult> Manage(ProfileViewModel vm)
    {
        
        var Account = context.Accounts.FirstOrDefault(c => c.AccountID == vm.ProfileModel.AccountID);
        if (Account != default(Account))
        {                
            Account.FirstName = vm.ProfileModel.FirstName;
            Account.LastName = vm.ProfileModel.LastName;
            Account.Phone = vm.ProfileModel.Phone;
            Account.Email = vm.ProfileModel.Email;
            Account.UnitId = vm.ProfileModel.UnitId;
            Account.DesignationId = vm.ProfileModel.DesignationId;
            Account.Id = vm.ProfileModel.Id;

        }

         try
         {
        //     if (ModelState.IsValid)
        //     {
                await context.SaveAccount(Account);
                return RedirectToAction("Index", "Account");
            // }
            // else
            // {
            //     return View(vm);
            // }
        }
        catch
        {
            return View(vm);
        }
    }

    
    public SelectList getUsers()
    {
        List<User> models = new List<User>();

        var AllUsers = from user in userManager.Users
                    select new
                    {
                        UserId = user.Id,
                        UserEmail = user.Email
                    };

        foreach (var item in AllUsers)
        {
            var m = new User();
            
            m.Id = item.UserId;
            m.Email = item.UserEmail;
            models.Add(m);
        }
        
        
        SelectList userSelect = new SelectList(models, "Id", "Email");
        return userSelect;
    }

    public SelectList getSubdepts()
    {
        List<Subdept> models = new List<Subdept>();

        var AllSubdepts = from unit in subdept_context.Subdepts
                select new
                {
                    UnitId = unit.UnitId,
                    SubdeptName = unit.SubdeptName
                };

        foreach (var item in AllSubdepts)
        {
            var m = new Subdept();
            
            m.UnitId = item.UnitId;
            m.SubdeptName = item.SubdeptName;
            models.Add(m);
        }
        
        
       SelectList userSelect = new SelectList(models, "UnitId", "SubdeptName");
       return userSelect;
    }

    public SelectList getDesignation()
    {       
        return DesignationSelectList = new SelectList(DesignationList, "Id", "Name");
    }

    // public IEnumerable<Designation> getDesignations()
    // {       
    //     return await Designation.ToList();
    // }

    [HttpPost]
    public ActionResult DeleteAccount(string Id)
    {        
        //Prepare Json Response
        JsonViewModel model = new JsonViewModel();
       
        if (Id.Length > 0)
        {            
            var Account = context.Accounts.Where(c => c.Id == Id).FirstOrDefault();

            model.ResponseCode = 0;
           // model.ResponseMessage = "Project Deleted successfully";
            string name = Account.FirstName + " "+ Account.LastName;
            if(name.Length >= 20)
            {
                model.ResponseMessage = JsonConvert.SerializeObject("Account "+ name.Substring(0, 15) + " was Deleted successfully.");
            }else{
                model.ResponseMessage = JsonConvert.SerializeObject("Account "+ name + " was Deleted successfully.");
            }

            Account = context.DeleteAccount(Id);
           
        }
        else
        {
            model.ResponseCode = 1;
            model.ResponseMessage = "Error deleting Account. No Id Found.";
        }

        return Json(model);
    }
  
}

