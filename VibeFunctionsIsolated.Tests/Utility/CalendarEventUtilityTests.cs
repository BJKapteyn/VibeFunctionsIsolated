using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Net;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Models.Cosmos;
using VibeFunctionsIsolated.Utility;

namespace VibeFunctionsIsolated.Tests.Utility;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CalendarEventUtilityTests
{
    private Mock<ICosmosDataAccess> cosmosDataAccessMock;
    private CalendarEventUtility cosmosUtility;

    [SetUp]
    public void SetUp()
    {
        cosmosDataAccessMock = new Mock<ICosmosDataAccess>();
        Mock<ILogger<CalendarEventUtility>> mockLogger = new();
        cosmosUtility = new CalendarEventUtility(cosmosDataAccessMock.Object, mockLogger.Object);
    }

    [Test]
    [TestCaseSource(nameof(UpsertCalendarEventTestCases))]
    public void UpsertCalendarEvent_StatusCodeTests(CalendarEvent calendarEvent, HttpStatusCode statusCode, bool expected)
    {
        // Arrange
        Mock<ItemResponse<CalendarEvent>> mockItemResponse = new ();
        mockItemResponse.Setup(x => x.StatusCode).Returns(statusCode);

        cosmosDataAccessMock.Setup(x => x.UpsertCosmosItemAsync(calendarEvent, null).Result).Returns(mockItemResponse.Object);
  
        // Act
        bool actual = cosmosUtility.UpsertCalendarEvent(calendarEvent).Result;

        // Assert
        Assert.That(actual == expected);

    }

    private static IEnumerable<TestCaseData> UpsertCalendarEventTestCases()
    {
        string eventDescription = "This is a test event description.";
        string eventName = "Test Event Name";
        DateTime startDate = DateTime.UtcNow;
        DateTime? endDate = DateTime.UtcNow.AddHours(1);
        string id = Guid.NewGuid().ToString();

        CalendarEvent anyEvent = new (id, eventName, eventDescription, startDate, endDate, id);

        yield return new TestCaseData(anyEvent, HttpStatusCode.OK, true);
        yield return new TestCaseData(anyEvent, HttpStatusCode.Created, true);
        yield return new TestCaseData(anyEvent, HttpStatusCode.NotFound, false);
        yield return new TestCaseData(anyEvent, HttpStatusCode.NotFound, false);
    }
}
