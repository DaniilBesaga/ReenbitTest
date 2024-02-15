using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Mail;
using Azure.Storage.Sas;
using Azure.Storage;
using SendGrid.Helpers.Mail;
using SendGrid;

namespace AzureFunction1
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([BlobTrigger("test/{name}/{email}", Connection = "")] Stream blob, string name, string email, ILogger log)
        {
            var blobSas = new BlobSasBuilder()
            {
                BlobContainerName = "test",
                BlobName = name,
                ExpiresOn = DateTime.UtcNow.AddHours(1),
            };
            var sasToken = blobSas.ToSasQueryParameters(new StorageSharedKeyCredential("test132312123121111", "Xm9q4PMgHFAGB8BiZFHxG/Ae9ORR7LMNrl50nCJbGOnNxQM7b8YGoJEvPrDjifF7/kY1UnODd5TR+AStgmCp1g==")).ToString();

            var emailSubject = "File Upload Notification";
            var emailBody = $"<p>The file {name} has been successfully uploaded. You can access it using the following link:</p>";
            var fileUrl = $"https://test132312123121111.blob.core.windows.net/test/{name}{sasToken}";
            emailBody += $"<a href='{fileUrl}'>{fileUrl}</a>";

            // Send email notification
            var senderEmail = new EmailAddress("daniilbesaga@gmail.com", "Daniel");
            var recipientEmail = new EmailAddress($"{email}", "");
            var message = MailHelper.CreateSingleEmail(senderEmail, recipientEmail, emailSubject, "", emailBody);
            var transportWeb = new SendGridClient("your-sendgrid-api-key");
            transportWeb.SendEmailAsync(message).GetAwaiter().GetResult();

            log.LogInformation($"File {name} processed successfully.");
        }
    }
}
