using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using VibeFunctionsIsolated.DAL.Interfaces;
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
}
