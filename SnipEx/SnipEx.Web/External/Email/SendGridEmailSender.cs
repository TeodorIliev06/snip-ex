namespace SnipEx.Web.External.Email
{
    using SendGrid;
    using SendGrid.Helpers.Mail;
    using Microsoft.AspNetCore.Identity.UI.Services;

    public class SendGridEmailSender(IConfiguration config) : IEmailSender
    {
        private readonly string apiKey = config.GetValue<string>("SendGrid:ApiKey")!;
        private readonly string fromEmail = config.GetValue<string>("SendGrid:FromEmail")!;
        private readonly string fromName = config.GetValue<string>("SendGrid:FromName")!;

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName);
            var to = new EmailAddress(email);

            var msg = MailHelper
                .CreateSingleEmail(from, to, subject, plainTextContent: null, htmlContent: htmlMessage);

            await client.SendEmailAsync(msg);
        }
    }
}
