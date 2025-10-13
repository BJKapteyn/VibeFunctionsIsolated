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
[Parallelizable(ParallelScope.Fixtures)]
public class CalendarEventUtilityTests
{
    private Mock<ICosmosDataAccess> cosmosDataAccessMock;
    private CalendarEventUtility calendarEventUtilityMock;

    [SetUp]
    public void SetUp()
    {
        cosmosDataAccessMock = new Mock<ICosmosDataAccess>();
        Mock<ILogger<CalendarEventUtility>> mockLogger = new();
        calendarEventUtilityMock = new CalendarEventUtility(cosmosDataAccessMock.Object, mockLogger.Object);
    }

    [Test]
    [TestCaseSource(nameof(UpsertCalendarEventTestCases))]
    public async Task UpsertCalendarEvent_StatusCodeTests(CalendarEvent calendarEvent, HttpStatusCode statusCode, bool expected)
    {
        // Arrange
        CosmosResponse itemResponse = new(statusCode, calendarEvent);

        cosmosDataAccessMock
            .Setup(x => x.UpsertCosmosItemAsync(It.IsAny<CalendarEvent>(), It.IsAny<string>()))
            .ReturnsAsync(itemResponse);

        // Act
        bool actual = await calendarEventUtilityMock.UpsertCalendarEvent(calendarEvent);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    private static IEnumerable<TestCaseData> UpsertCalendarEventTestCases()
    {
        // Use fixed values for dates and IDs
        string squareEventId = "squareEventId";
        string squareVariationId = "squareVariationId";
        long squareEventVersion = 1;
        string eventDescription = "This is a test event description.";
        string eventName = "Test Event Name";
        string organizerName = "Test Organizer";
        DateTime startDate = new DateTime(2024, 1, 1);
        DateTime? endDate = new DateTime(2024, 1, 1, 1, 0, 0);
        string bannerImageUrl = "";
        string id = "fixed-id";
        long priceInUSD = 2500;
        const bool expectedTrue = true;
        const bool expectedFalse = false;

        yield return new TestCaseData(
            new CalendarEvent(squareEventId, squareVariationId, squareEventVersion, eventName, eventDescription, startDate, endDate, organizerName, bannerImageUrl, priceInUSD, id),
            HttpStatusCode.OK,
            expectedTrue
        );
        yield return new TestCaseData(
            new CalendarEvent(squareEventId, squareVariationId, squareEventVersion, eventName, eventDescription, startDate, endDate, organizerName, bannerImageUrl, priceInUSD, id),
            HttpStatusCode.Created,
            expectedTrue
        );
        yield return new TestCaseData(
            new CalendarEvent(squareEventId, squareVariationId, squareEventVersion, eventName, eventDescription, startDate, endDate, organizerName, bannerImageUrl, priceInUSD, id),
            HttpStatusCode.NotFound,
            expectedFalse
        );
    }
}
