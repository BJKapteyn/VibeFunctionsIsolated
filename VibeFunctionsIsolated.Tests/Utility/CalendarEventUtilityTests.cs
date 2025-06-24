using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Models.Cosmos;
using VibeFunctionsIsolated.Utility;

namespace VibeFunctionsIsolated.Tests.Utility;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CalendarEventUtilityTests
{
    private Mock<ICosmosDataAccess> cosmosDataAccessMock;
    private CosmosCalendarEventUtility cosmosUtility;

    [SetUp]
    public void SetUp()
    {
        cosmosDataAccessMock = new Mock<ICosmosDataAccess>();
        Mock<ILogger<CosmosCalendarEventUtility>> mockLogger = new Mock<ILogger<CosmosCalendarEventUtility>>();
        cosmosUtility = new CosmosCalendarEventUtility(cosmosDataAccessMock.Object, mockLogger.Object);
    }

    [Test]
    public void UpsertCalendarEventTests()
    {
        // Arrange
        var calendarEvent = new CalendarEvent { id = "test-id" };
        cosmosDataAccessMock.Setup(x => x.UpsertCosmosItemAsync(calendarEvent, calendarEvent.id))
            .ReturnsAsync(new ItemResponse<CalendarEvent>(calendarEvent, System.Net.HttpStatusCode.OK, null, null, null));
        // Act
        var result = cosmosUtility.UpsertCalendarEvent(calendarEvent).Result;
        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("test-id", result.id);
        cosmosDataAccessMock.Verify(x => x.UpsertCosmosItemAsync(calendarEvent, calendarEvent.id), Times.Once);
    }

    public static TestCaseData[] UpsertCalendarEventTestCases()
    {
        CalendarEvent goodId = new CalendarEvent
        {
            id = "good-id",
            EventId = "event-123",
            EventName = "Test Event",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddHours(1)
        };

        return new[]
        {
            new TestCaseData(new CalendarEvent { id = "test-id" }, System.Net.HttpStatusCode.OK),
            new TestCaseData(new CalendarEvent { id = "new-id" }, System.Net.HttpStatusCode.Created),
            new TestCaseData(new CalendarEvent { id = "not-found-id" }, System.Net.HttpStatusCode.NotFound)
        };
    }
}
