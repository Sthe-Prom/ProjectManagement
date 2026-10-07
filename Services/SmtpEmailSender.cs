using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using ProjectManagement.Models;

namespace ProjectManagement.Services
{
    public sealed class SmtpEmailSender(IOptions<SmtpEmailOptions> options) : IEmailSender
    {
        private readonly SmtpEmailOptions _options = options.Value;

        public async Task SendAsync(
            string email,
            string subject,
            string htmlMessage,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_options.Host) ||
                string.IsNullOrWhiteSpace(_options.FromAddress))
            {
                throw new InvalidOperationException(
                    "SMTP Host and FromAddress must be configured before sending email.");
            }

            if (string.IsNullOrWhiteSpace(_options.UserName) !=
                string.IsNullOrWhiteSpace(_options.Password))
            {
                throw new InvalidOperationException(
                    "Configure both SMTP UserName and Password, or leave both empty.");
            }

            if (!_options.UseSsl &&
                !_options.UseImplicitSsl &&
                !string.IsNullOrWhiteSpace(_options.UserName))
            {
                throw new InvalidOperationException(
                    "SMTP authentication requires TLS to protect credentials.");
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlMessage }.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _options.Host,
                _options.Port,
                _options.UseImplicitSsl
                    ? SecureSocketOptions.SslOnConnect
                    : _options.UseSsl
                        ? SecureSocketOptions.StartTls
                        : SecureSocketOptions.None,
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(_options.UserName))
            {
                await client.AuthenticateAsync(
                    _options.UserName,
                    _options.Password,
                    cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
    }
}
