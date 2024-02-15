namespace TestProject2
{
    public class UnitTest1
    {
        [Fact]
        public async Task Upload_ValidItem_ReturnsOkResult()
        {
            // Arrange
            var mockItemRepository = new Mock<I_ItemRepository>();
            var controller = new ItemController(mockItemRepository.Object);
            var item = new Item();

            // Act
            var result = await controller.Upload(item) as OkResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }
    }
}