namespace AzureSendMail
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using FluentValidation.Results;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using SendGrid;
    using SendGrid.Helpers.Mail;

    /// <summary>
    /// A simple Azure function to send mail from static website
    /// </summary>
    public static class SendMail
    {
        [FunctionName("SendMail")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            ILogger log)
        {
            if (req.Body == null)
            {
                log.LogWarning("Received a send mail request with an null body");
                return new BadRequestObjectResult("Mail body is null");
            }

            using var reader = new StreamReader(req.Body, Encoding.UTF8);

            var serializer = JsonSerializer.Create();
            MailData mailData = (MailData)serializer.Deserialize(reader, typeof(MailData));
            MailDataValidator mailDataValidator = new MailDataValidator();
            ValidationResult result = mailDataValidator.Validate(mailData);

            if (!result.IsValid)
            {
                log.LogError("Received invalid mail data");

                StringBuilder stringBuilder = new();

                foreach (var error in result.Errors)
                {
                    stringBuilder.Append($"{error.ErrorMessage}; ");
                }

                return new BadRequestObjectResult(stringBuilder.ToString());
            }

            var response = await SendMailWithSendGrid(mailData);

            if (response != null)
            {
                log.LogInformation($"Response received from SendGrid. StatusCode: {(int)response.StatusCode}.");
                return new StatusCodeResult((int)response.StatusCode);
            }
            else
            {
                log.LogError($"Function not correctly configured. Mission API KEY e/o RECIPIENT.");
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// Send an email using the SendGrid service
        /// </summary>
        /// <param name="mailData">the <see cref="MailData"/></param>
        /// <returns>A response object</returns>
        private static Task<Response> SendMailWithSendGrid(MailData mailData)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_APIKEY")?.Trim();
            var mailSender = Environment.GetEnvironmentVariable("SENDGRID_FROMMAIL")?.Trim();
            var recipient = Environment.GetEnvironmentVariable("SENDGRID_RECIPIENT").Trim();

            if (string.IsNullOrEmpty(mailSender) ||
                string.IsNullOrEmpty(apiKey) ||
                string.IsNullOrEmpty(recipient))
            {
                return Task.FromResult<Response>(null);                    
            }

            SendGridMessage email = new();
            email.From = new EmailAddress(mailSender, "M1K Mailer");
            email.AddTo(recipient);
            email.AddReplyTo(mailData.FromMail);
            email.Subject = $"[M1K WebSite]: {mailData.Subject.Normalize(NormalizationForm.FormKD)}";

            var message = $"Sender: {mailData.FromName}\nEmail Address: {mailData.FromMail}\n\nMessage:\n{mailData.Message}";
            email.PlainTextContent = message.Normalize(NormalizationForm.FormKD);

            var client = new SendGridClient(apiKey);

            return client.SendEmailAsync(email);
        }
    }
}