using Azure.Storage.Blobs;
using FunctionApp2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ReenbitTest.Server.Controllers;
using ReenbitTest.Server.Interfaces;
using ReenbitTest.Server.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestProject
{
    public class Tests
    {
        private Mock<I_ItemRepository> _mockItemRepository;
        private ItemController _itemController;
        private Mock<ILogger<Function1>> _mockLogger;

        [SetUp]
        public void SetUp()
        {
            _mockItemRepository = new Mock<I_ItemRepository>();
            _itemController = new ItemController(_mockItemRepository.Object);
            _mockLogger = new Mock<ILogger<Function1>>();
            _mockItemRepository.Setup(x => x.GetAllItemsAsync()).ReturnsAsync(GetFakeItemsList);
        }

        [Test]
        public async Task Upload_ValidItem_ReturnsOkResult()
        {
            // Arrange   
            var item = new Item();

            // Act
            var result = await _itemController.Upload(item) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }
        public async Task Upload_NullItem_ReturnsBadRequestResult()
        {
            // Arrange

            // Act
            var result = await _itemController.Upload(null) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task Download_ValidName_ReturnsFileResult()
        {
            // Arrange
            string name = "File 1.docx";
            byte[] fakeFileContent = GetFakeFileContent();
            var fakeItemStream = new MemoryStream(fakeFileContent);

            _mockItemRepository.Setup(x => x.GetItemByNameAsync(name)).ReturnsAsync(fakeItemStream);

            // Act
            var result = await _itemController.Download(name) as FileResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("document/docx", result.ContentType);
            Assert.AreEqual(name, result.FileDownloadName);
        }


        [Test]
        public async Task Download_InvalidName_ReturnsBadRequestResult()
        {
            // Arrange
            string name = "invalid_filename";
            _mockItemRepository.Setup(x => x.GetItemByNameAsync(name)).Throws(new Exception("Item not found"));
            // Act
            var result = await _itemController.Download(name) as BadRequestObjectResult;
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task Delete_ValidName_ReturnsOkResult()
        {
            // Arrange
            string name = "filename";

            // Act
            var result = await _itemController.Delete(name) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task Delete_InvalidName_ReturnsBadRequestResult()
        {
            // Arrange
            string name = "##!@!#$%";
            _mockItemRepository.Setup(x => x.DeleteItemAsync(name)).Throws(new Exception("Item not found"));
            // Act
            var result = await _itemController.Delete(name) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task ListAllBlobs_ReturnsOkResultWithData()
        {
            // Arrange
            
            // Act
            var result = await _itemController.ListAllBlobs() as OkObjectResult;
            var resultData = result.Value as List<string>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsNotNull(resultData);
            Assert.AreEqual(2, resultData.Count);
        }
        
        private static List<string> GetFakeItemsList()
        {
            return new List<string>
            {
                "Practice 7.docx", "File 1.docx"
            };
        }

        byte[] GetFakeFileContent()
        {
            List<byte> content = new List<byte>();

            Random random = new Random();
            for (int i = 0; i < 20000; i++)
            {
                byte randomByte = (byte)random.Next(0, 256);
                content.Add(randomByte);
            }

            return content.ToArray();
        }

        [Test]
        public void Run_ValidBlobAndMetadata_SuccessfullyProcesses()
        {
            // Arrange
            var blobContent = "Test blob content";
            var blobName = "testBlob.docx";
            var metadata = new Dictionary<string, string>
            {
                { "email", "test@example.com" }
            };

            var loggerFactoryMock = new Mock<ILoggerFactory>();
            loggerFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_mockLogger.Object);

            var function = new Function1(_mockLogger.Object);

            // Act
            function.Run(blobContent, blobName, metadata);

            // Assert
            _mockLogger.Verify(x => x.Log(
            It.Is<LogLevel>(l => l == LogLevel.Information),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => VerifyLogMessageContains(v.ToString(), blobName)),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
            ), Times.Once);
        }
        bool VerifyLogMessageContains(string logMessage, string expectedBlobName)
        {
            return logMessage.Contains($"File {expectedBlobName} processed successfully.");
        }
    }

}
