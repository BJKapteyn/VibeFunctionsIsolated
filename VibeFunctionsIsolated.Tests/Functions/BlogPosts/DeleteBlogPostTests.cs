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
using VibeFunctionsIsolated.Models.Cosmos;
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
            IActionResult expected)
        {
            // Arrange
            var mockRequest = new Mock<HttpRequest>();
            // Simulate request body containing the blog post id
            appUtility.Setup(x => x.DeserializeStream<CosmosItemId>(It.IsAny<Stream>()).Result).Returns(deserializedRequest);
           

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
            var okResult = new OkObjectResult(new object());
            var notFoundResult = new NotFoundObjectResult(new object());

            // Blog post found and deleted
            yield return new TestCaseData(validCosmosItemId, okResult);
            // Blog post not found
            yield return new TestCaseData(invalidCosmosItemId, notFoundResult);
            yield return new TestCaseData(nullCosmosItemId, notFoundResult);
        }
    }
}
