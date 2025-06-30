using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ProjectManagement.Views.Account
{
    public class Manage : PageModel
    {
        private readonly ILogger<Manage> _logger;

        public Manage(ILogger<Manage> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}