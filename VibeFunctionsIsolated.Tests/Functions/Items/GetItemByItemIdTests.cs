using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Square.Models;
using VibeFunctionsIsolated.DAL;
using VibeFunctionsIsolated.Functions.Items;
using VibeFunctionsIsolated.Models;
using VibeFunctionsIsolated.Utility;

namespace VibeFunctionsIsolated.Tests.Functions.Items;

[TestFixture]
[Parallelizable]
public class GetItemByItemIdTests
{
    private Mock<ILogger<GetItemByItemId>> logger;
    private Mock<ISquareUtility> squareUtility;
    private Mock<ISquareDAL> squareDAL;

    [SetUp]
    public void Setup()
    {
        logger = new Mock<ILogger<GetItemByItemId>>();
        squareUtility = new Mock<ISquareUtility>();
        squareDAL = new Mock<ISquareDAL>();
    }

    [Test]
    [Parallelizable]
    [TestCaseSource(nameof(GetItemByItemIdCorrectResponseTestCases))]
    public async Task GetItemByItemId_CorrectResponseTest(CatalogObject? squareResponseMObject, ItemId? requestBody, IActionResult expected)
    {
        // Arrange
        CatalogObject? populatedResponseBody = new CatalogObject("ITEM", Guid.NewGuid().ToString());
        RetrieveCatalogObjectResponse squareResponse = new(mObject: squareResponseMObject);
        squareUtility.Setup(utility => utility.DeserializeStream<ItemId>(It.IsAny<Stream>())).ReturnsAsync(requestBody);
        squareDAL.Setup(dal => dal.GetCatalogObjectById(It.IsAny<ItemId>())).ReturnsAsync(squareResponse);
        Mock<HttpRequest> mockRequest = new();

        GetItemByItemId function = new GetItemByItemId(logger.Object, squareUtility.Object, squareDAL.Object);

        // Act
        IActionResult actual = await function.Run(mockRequest.Object);

        // Assert
        Assert.That(actual.GetType(), Is.EqualTo(expected.GetType()));

    }

    public static IEnumerable<TestCaseData> GetItemByItemIdCorrectResponseTestCases()
    {
        ItemId? goodRequestId = new("GoodId");
        ItemId? nullRequestId = null;

        CatalogObject? populatedResponseBody = new CatalogObject("ITEM", "itemId", itemData: new CatalogItem());
        CatalogObject? emptyResponseBody = null;

        BadRequestResult badRequestResult = new();
        NotFoundResult notFoundResult = new();
        OkObjectResult okObjectResult = new(new List<SquareCategory>());

        yield return new TestCaseData(emptyResponseBody, goodRequestId, notFoundResult);
        yield return new TestCaseData(emptyResponseBody, nullRequestId, badRequestResult);
        yield return new TestCaseData(populatedResponseBody, goodRequestId, okObjectResult);
    }
}
