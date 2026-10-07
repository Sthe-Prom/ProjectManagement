using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models
{
    public sealed record ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; init; } = string.Empty;
    }

    public sealed record ResetPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; init; } = string.Empty;

        [Required]
        [StringLength(8192)]
        public string Code { get; init; } = string.Empty;

        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        public string Password { get; init; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; init; } = string.Empty;
    }

    public sealed class SmtpEmailOptions
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromAddress { get; set; } = string.Empty;
        public string FromName { get; set; } = "MIR Project Management";
        public bool UseSsl { get; set; } = true;
        public bool UseImplicitSsl { get; set; }
    }

    public sealed class PasswordResetOptions
    {
        public string PublicBaseUrl { get; set; } = string.Empty;
    }
}
