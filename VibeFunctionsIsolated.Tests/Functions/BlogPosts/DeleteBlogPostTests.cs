using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VibeFunctionsIsolated.Functions.BlogPosts;
using VibeFunctionsIsolated.Functions.CalendarEvents;
using VibeFunctionsIsolated.Models.Cosmos;
using VibeFunctionsIsolated.Models.Cosmos.UtilityModels;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Tests.Functions.BlogPosts
{
    [TestFixture]
    [Parallelizable]
    public class DeleteBlogPostTests
    {
        private Mock<ILogger<DeleteBlogPostById>> logger;
        private Mock<IBlogPostUtility> blogPostUtility;
        private Mock<IApplicationUtility> appUtility;

        [SetUp]
        public void Setup()
        {
            logger = new Mock<ILogger<DeleteBlogPostById>>();
            blogPostUtility = new Mock<IBlogPostUtility>();
            appUtility = new Mock<IApplicationUtility>();
        }

        [Test]
        [Parallelizable]
        [TestCaseSource(nameof(DeleteBlogPostTestCases))]
        public async Task DeleteBlogPostById_CorrectResponseTest(
            CosmosItemId deserializedRequest,
            bool didDelete,
            IActionResult expected)
        {
            // Arrange
            var mockRequest = new Mock<HttpRequest>();
            // Simulate request body containing the blog post id
            appUtility.Setup(x => x.DeserializeStream<CosmosItemId>(It.IsAny<Stream>()).Result).Returns(deserializedRequest);
            blogPostUtility.Setup(utility => utility.DeleteBlogPost(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(didDelete);

            var deleteBlogPostById = new DeleteBlogPostById(logger.Object, blogPostUtility.Object, appUtility.Object);

            // Act
            IActionResult actual = await deleteBlogPostById.Run(mockRequest.Object);

            // Assert
            Assert.That(actual.GetType(), Is.EqualTo(expected.GetType()));
        }

        private static IEnumerable<TestCaseData> DeleteBlogPostTestCases()
        {
            BlogPost validBlogPost = new ("id123", "Title", "Author", System.DateTime.UtcNow, "Content", null);
            CosmosItemId? validCosmosItemId = new("id123", "partitionKey");
            CosmosItemId? invalidCosmosItemId = new("", "partitionKey");
            CosmosItemId? nullCosmosItemId = null;
            bool didDelete = true;
            bool didNOTDelete = false;
            var okResult = new OkObjectResult(new object());
            var notFoundResult = new NotFoundObjectResult(new object());

            yield return new TestCaseData(validCosmosItemId, didDelete, okResult);
            yield return new TestCaseData(invalidCosmosItemId, didNOTDelete, notFoundResult);
            yield return new TestCaseData(nullCosmosItemId, didNOTDelete, notFoundResult);
        }
    }
}
