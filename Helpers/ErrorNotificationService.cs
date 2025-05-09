using Generic.Helpers.Utility;
using Generic.Models;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Generic.Helpers
{
    public class ErrorNotificationService : IErrorNotificationService
    {
        private readonly ILoggingService _logger;
        private readonly string[] _adminEmails;
        private EntitiesDBContext db = new EntitiesDBContext();

        public ErrorNotificationService(ILoggingService logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var emails = ConfigurationManager.AppSettings["ErrorNotification_AdminEmails"];
            _adminEmails = string.IsNullOrEmpty(emails)
                ? new string[0]
                : emails.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public void NotifyAdministrators(Exception exception, string additionalInfo = null)
        {
            try
            {

                enterprise objEnterprise = db.pr_getEnterprise(1).FirstOrDefault();
                if (_adminEmails.Length == 0)
                {
                    _logger.LogWarning("No admin emails configured for error notifications");
                    return;
                }

                var subject = $"Critical Error: {exception.GetType().Name}";

                var body = new StringBuilder();
                body.AppendLine("<h2>Critical Error Notification</h2>");
                body.AppendLine($"<p><strong>Time:</strong> {DateTime.Now}</p>");
                body.AppendLine($"<p><strong>Exception Type:</strong> {exception.GetType().FullName}</p>");
                body.AppendLine($"<p><strong>Message:</strong> {exception.Message}</p>");

                if (!string.IsNullOrEmpty(additionalInfo))
                {
                    body.AppendLine($"<p><strong>Additional Information:</strong> {additionalInfo}</p>");
                }

                body.AppendLine("<h3>Stack Trace:</h3>");
                body.AppendLine($"<pre>{exception.StackTrace}</pre>");

                if (exception.InnerException != null)
                {
                    body.AppendLine("<h3>Inner Exception:</h3>");
                    body.AppendLine($"<p><strong>Type:</strong> {exception.InnerException.GetType().FullName}</p>");
                    body.AppendLine($"<p><strong>Message:</strong> {exception.InnerException.Message}</p>");
                    body.AppendLine($"<pre>{exception.InnerException.StackTrace}</pre>");
                }

                var autoMail = new autoMailMessage
                {
                    subject = subject,
                    text = body.ToString()
                };

                var mail = new Email(autoMail)
                {
                    type = "emailAlert",
                    emailTo = string.Join(";", _adminEmails),
                    category = SendGridCategory.SendEmailAlert,
                    subject = subject,
                    body = body.ToString()
                };

                var objSendEmail = new SendEmail();
                objSendEmail.sendEmail(mail, new EmailFormatSettings
                {
                    enterprise = objEnterprise,
                    sender = null,
                    touchpoint = null
                });

                _logger.LogInformation("Sent error notification email to administrators");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send error notification email");
            }
        }
    }


}