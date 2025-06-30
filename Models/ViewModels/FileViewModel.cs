using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProjectManagement.Models;
using Microsoft.AspNetCore.Http;

namespace ProjectManagement.Models
{
    public class FileViewModel
    {
        public IFormFile ProjectFiles { get; set; }
    }
}