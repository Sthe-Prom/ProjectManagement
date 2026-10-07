using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Interfaces;
using ProjectManagement.Services;
using ProjectManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ProjectManagement.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
          IConfiguration config)
        {
            //iFrame Fix
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("https://mir-projects2.powerappsportals.com/")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .AllowCredentials());
            });

            //Context Identity
            services.AddDbContext<AppIdentityDbContext>(opt =>
            {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
                //opt.UseSqlServer(config.GetConnectionString("Prod"));
            });

            //Identity Service
            services.AddIdentity<User, IdentityRole>(opt =>
            {
                //Username Options
                opt.User.RequireUniqueEmail = true;

                //Password Options
                opt.Password.RequiredLength = 6;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireDigit = false;

            }).AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();

            services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
                //opt.UseSqlServer(config.GetConnectionString("Prod"));
            });

            //MVC - Route Config
            services.AddMvc(options => options.EnableEndpointRouting = false);

            //Configure Antiforgery options(i - Frame Spooky Repeller);
            services.AddAntiforgery(options =>
            {
                options.SuppressXFrameOptionsHeader = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                //options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                //options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

            services.Configure<SmtpEmailOptions>(config.GetSection("Smtp"));
            services.Configure<PasswordResetOptions>(config.GetSection("PasswordReset"));
            services.AddTransient<IEmailSender, SmtpEmailSender>();

            //Application Service Registration
            services.AddTransient<IProject, EFProject>();
            services.AddTransient<IProjectAction, ProjectService>();
            services.AddTransient<IActivity, EFActivity>();
            services.AddTransient<IStatus, EFStatus>();
            services.AddTransient<ISubdept, EFSubdept>();
            services.AddTransient<IAccount, EFAccount>();
            services.AddTransient<IProjectType, EFProjectType>();
            services.AddTransient<IKPI, EFKPI>();
           
            return services;
        }
    }
}