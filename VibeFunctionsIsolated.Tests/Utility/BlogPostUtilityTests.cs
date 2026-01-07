using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Net;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Models.Cosmos;
using VibeFunctionsIsolated.Utility;

namespace VibeFunctionsIsolated.Tests.Utility;

[TestFixture]
[Parallelizable(ParallelScope.Fixtures)]
public class BlogPostUtilityTests
{
    private Mock<ICosmosDataAccess> cosmosDataAccessMock;
    private BlogPostUtility blogPostUtility;
    private Mock<ILogger<BlogPostUtility>> loggerMock;

    [SetUp]
    public void SetUp()
    {
        cosmosDataAccessMock = new Mock<ICosmosDataAccess>();
        loggerMock = new Mock<ILogger<BlogPostUtility>>();
        blogPostUtility = new BlogPostUtility(cosmosDataAccessMock.Object, loggerMock.Object);
    }

    [Test]
    [TestCaseSource(nameof(GetBlogPostTestCases))]
    public async Task GetBlogPost_StatusCodeTests(BlogPost? expected, HttpStatusCode statusCode)
    {
        // Arrange
        const string id = "id123";
        const string partitionKey = "partition";

        var cosmosResponse = new CosmosResponse(statusCode, expected);

        cosmosDataAccessMock
            .Setup(x => x.GetItemWithStatusCodeAsync<BlogPost>(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(cosmosResponse);

        // Act
        var actual = await blogPostUtility.GetBlogPost(id, partitionKey);

        // Assert
        Assert.That(actual?.id, Is.EqualTo(expected?.id));
        loggerMock.Verify(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception?>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeast(1));
    }

    private static IEnumerable<TestCaseData> GetBlogPostTestCases()
    {
        var validBlogPost = new BlogPost("id123", "Title", "Author", DateTime.UtcNow, "Content", null);
        var wrongTypeCalendarEvent = new CalendarEvent("squareEventId", "squareVariationId", 1, "EventName", "Description", DateTime.UtcNow, null, "Organizer", "", 1000, "id123");

        // OK -> returns BlogPost
        yield return new TestCaseData(validBlogPost, HttpStatusCode.OK);

        // NotFound
        yield return new TestCaseData(null, HttpStatusCode.NotFound);

        // Unauthorized
        yield return new TestCaseData(null, HttpStatusCode.Unauthorized);

        // OK but wrong item type
        yield return new TestCaseData(null, HttpStatusCode.OK);
    }

    [Test]
    public async Task GetBlogPost_WhenDataAccessThrows_ReturnsNullAndDoesNotThrow()
    {
        // Arrange
        const string id = "id123";
        const string partitionKey = "partition";

        cosmosDataAccessMock
            .Setup(x => x.GetItemWithStatusCodeAsync<BlogPost>(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Cosmos failure"));

        // Act
        var actual = await blogPostUtility.GetBlogPost(id, partitionKey);

        // Assert
        Assert.That(actual, Is.Null);
    }
}
