using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using ProjectManagement.Models;
using ProjectManagement.Services;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Text.Encodings.Web;

namespace ProjectManagement.Controllers
{
    public class UserAccountController: Controller
    {
          //private properties
        private UserManager<User> UserManager;
        private SignInManager<User> signInManager;
        private readonly IEmailSender emailSender;
        private readonly PasswordResetOptions passwordResetOptions;
        private readonly ILogger<UserAccountController> logger;

        //Const
        public UserAccountController(
            UserManager<User> userManager_,
            SignInManager<User> signInManager_,
            IEmailSender emailSender_,
            IOptions<PasswordResetOptions> passwordResetOptions_,
            ILogger<UserAccountController> logger_)
        {
            UserManager = userManager_;
            signInManager = signInManager_;
            emailSender = emailSender_;
            passwordResetOptions = passwordResetOptions_.Value;
            logger = logger_;
        }

        //Action Methods      
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordModel());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(
            ForgotPasswordModel model,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User? user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                logger.LogInformation(
                    "Password reset requested, but no user account matched the submitted email.");
                return View("ForgotPasswordConfirmation");
            }

            try
            {
                if (!Uri.TryCreate(
                        passwordResetOptions.PublicBaseUrl,
                        UriKind.Absolute,
                        out Uri? publicBaseUri) ||
                    publicBaseUri == null ||
                    (publicBaseUri.Scheme != Uri.UriSchemeHttps &&
                     !(publicBaseUri.Scheme == Uri.UriSchemeHttp && publicBaseUri.IsLoopback)))
                {
                    throw new InvalidOperationException(
                        "PasswordReset:PublicBaseUrl must use HTTPS, except for localhost development.");
                }

                string token = await UserManager.GeneratePasswordResetTokenAsync(user);
                string code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                string? resetPath = Url.Action(
                    nameof(ResetPassword),
                    "UserAccount",
                    new { code, email = model.Email },
                    publicBaseUri.Scheme,
                    publicBaseUri.Authority);

                if (resetPath == null)
                {
                    throw new InvalidOperationException("Could not generate the password reset URL.");
                }

                string resetUrl = HtmlEncoder.Default.Encode(resetPath);
                string htmlMessage =
                    "<p>We received a request to reset your MIR Project Management password.</p>" +
                    $"<p><a href=\"{resetUrl}\">Reset your password</a></p>" +
                    "<p>If you did not request this, you can ignore this email.</p>";

                await emailSender.SendAsync(
                    model.Email,
                    "Reset your MIR Project Management password",
                    htmlMessage,
                    cancellationToken);

                logger.LogInformation(
                    "SMTP accepted the password reset message for a matched user account.");
            }
            catch (Exception ex) when (
                ex is InvalidOperationException ||
                ex is MailKit.ServiceNotConnectedException ||
                ex is MailKit.ServiceNotAuthenticatedException ||
                ex is MailKit.Net.Smtp.SmtpCommandException ||
                ex is MailKit.Net.Smtp.SmtpProtocolException ||
                ex is MailKit.Security.AuthenticationException ||
                ex is MailKit.Security.SslHandshakeException ||
                ex is SocketException ||
                ex is IOException ||
                ex is AuthenticationException ||
                ex is FormatException ||
                ex is ArgumentException)
            {
                logger.LogError(ex, "Could not send a password reset email.");
            }

            // Keep the response identical for known and unknown addresses to prevent account enumeration.
            return View("ForgotPasswordConfirmation");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string? code, string? email)
        {
            return View(new ResetPasswordModel
            {
                Code = code ?? string.Empty,
                Email = email ?? string.Empty
            });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(
            ResetPasswordModel model,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User? user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "The password reset link is invalid or expired.");
                return View(model);
            }

            string token;
            try
            {
                token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));
            }
            catch (FormatException)
            {
                ModelState.AddModelError(string.Empty, "The password reset link is invalid or expired.");
                return View(model);
            }

            IdentityResult result = await UserManager.ResetPasswordAsync(user, token, model.Password);
            if (result.Succeeded)
            {
                TempData["PasswordResetMessage"] = "Your password has been reset. You can now sign in.";
                return RedirectToAction(nameof(Login));
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                User user = await UserManager.FindByEmailAsync(loginModel.Email);
                var userId = UserManager.GetUserId(User);
                User user_id = await UserManager.FindByIdAsync(userId);

                if (user != null)
                {
                    try
                    {
                        Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(user, loginModel.Password, false, false);

                        if (result.Succeeded)
                        {
                            return Redirect("/Home/Index");
                        }
                        else
                        {
                            return Redirect("/UserAccount/Login");
                            //return Content('could not login');
                        }
                    }
                    catch (Exception ex)
                    {
                        return Content(ex.Message);
                    }

                }
                // else
                // {

                    ModelState.AddModelError(nameof(LoginModel.Email), "Invalid username or password");
                    //return Content("Model is Valid.");
                   // return View(loginModel);
                //}
            }                        

            //ModelState.AddModelError(nameof(LoginModel.Email), "Invalid username or password");
            return View(loginModel);

        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "UserAccount");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}