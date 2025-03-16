using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Functions.Items;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Tests.Functions.Events;

[TestFixture]
[Parallelizable]
internal class UpsertEventTests
{
    private Mock<ILogger<GetCategoriesByCategoryId>> logger;
    private Mock<ICosmosDataAccess> ICosmosDataAccess;
    private Mock<IApplicationUtility> appUtility;

    [SetUp]
    public void Setup()
    {
        logger = new Mock<ILogger<GetCategoriesByCategoryId>>();
        ICosmosDataAccess = new Mock<ICosmosDataAccess>();
        appUtility = new Mock<IApplicationUtility>();
    }
}
