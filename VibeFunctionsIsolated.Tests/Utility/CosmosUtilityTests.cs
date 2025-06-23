using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Utility;

namespace VibeFunctionsIsolated.Tests.Utility;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CosmosUtilityTests
{
    private Mock<ICosmosDataAccess> cosmosDataAccessMock;
    private CosmosCalendarEventUtility cosmosUtility;

    [SetUp]
    public void SetUp()
    {
        cosmosDataAccessMock = new Mock<ICosmosDataAccess>();
        cosmosUtility = new CosmosUtility(cosmosDataAccessMock.Object);
    }

    [Test]
}
