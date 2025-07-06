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
        Mock<ILogger<CosmosCalendarEventUtility>> mockLogger = new();
        cosmosUtility = new CosmosCalendarEventUtility(cosmosDataAccessMock.Object, mockLogger.Object);
    }

    [Test]
    public void UpsertCalendarEventTests()
    {
        // Arrange
  
        // Act

        // Assert
     
    }

    private static TestCaseData[] UpsertCalendarEventTestCases()
    {

        return new[]
        {
            new TestCaseData(new CalendarEvent { id = "test-id" }, System.Net.HttpStatusCode.OK),
            new TestCaseData(new CalendarEvent { id = "new-id" }, System.Net.HttpStatusCode.Created),
            new TestCaseData(new CalendarEvent { id = "not-found-id" }, System.Net.HttpStatusCode.NotFound)
        };
    }
}
