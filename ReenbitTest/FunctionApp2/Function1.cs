using System;
using System.IO;
using Azure.Storage;
using Azure.Storage.Sas;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using Azure.Storage.Blobs;
using System.Reflection.Metadata;

namespace FunctionApp2
{
    public class Function1
    {
        private readonly ILogger _logger;

        public Function1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
        }
        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("Function1")]
        public void Run([BlobTrigger("test/{name}", Connection = "")] string myBlob, string name, IDictionary<string, string> metadata)
        {
            _logger.LogInformation($"File {name} processed successfully.");
            if (metadata.Count > 0)
            {
                const string AccountName = "test132312123121111";
                const string AccountKey = "Xm9q4PMgHFAGB8BiZFHxG/Ae9ORR7LMNrl50nCJbGOnNxQM7b8YGoJEvPrDjifF7/kY1UnODd5TR+AStgmCp1g==";
                const string ContainerName = "test";
                string BlobName = $"{name}";
                const string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=test132312123121111;AccountKey=Xm9q4PMgHFAGB8BiZFHxG/Ae9ORR7LMNrl50nCJbGOnNxQM7b8YGoJEvPrDjifF7/kY1UnODd5TR+AStgmCp1g==;EndpointSuffix=core.windows.net";

                BlobContainerClient blobContainerClient = new BlobContainerClient(ConnectionString,
                    ContainerName);

                BlobClient blobClient = blobContainerClient.GetBlobClient(BlobName);

                Azure.Storage.Sas.BlobSasBuilder blobSasBuilder = new Azure.Storage.Sas.BlobSasBuilder()
                {
                    BlobContainerName = ContainerName,
                    BlobName = $"{name}",
                    ExpiresOn = DateTime.UtcNow.AddHours(1),
                };
                blobSasBuilder.SetPermissions(Azure.Storage.Sas.BlobSasPermissions.Read);
                var sasToke = blobSasBuilder.ToSasQueryParameters(new
                StorageSharedKeyCredential(AccountName, AccountKey)).ToString();
                var sasURL = $"{blobClient.Uri.AbsoluteUri}?{sasToke}";


                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("novella14@ethereal.email"));
                email.To.Add(MailboxAddress.Parse($"{metadata["email"]}"));
                email.Subject = "File Upload Notification";
                var fileUrl = $"https://test132312123121111.blob.core.windows.net/test/{name}/{sasToke}";
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = $"<p>The file {name} has been successfully uploaded. You can access it using the following link:</p><br><a href='{sasURL}'>{sasURL}</a>" };
                using var smtp = new SmtpClient();
                smtp.Connect("smtp.ethereal.email", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate("novella14@ethereal.email", "wa5DHkRyB8jMV72cYN");
                smtp.Send(email);
                smtp.Disconnect(true);       

            }
            
        }
    }
}
