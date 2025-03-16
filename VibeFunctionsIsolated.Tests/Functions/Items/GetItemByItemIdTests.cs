using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Square.Models;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Functions.Items;
using VibeFunctionsIsolated.Models.Square;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Tests.Functions.Items;

[TestFixture]
[Parallelizable]
public class GetItemByItemIdTests
{
    private Mock<ILogger<GetItemByItemId>> logger;
    private Mock<ISquareUtility> squareUtility;
    private Mock<ISquareSdkDataAccess> squareDAL;
    private Mock<IApplicationUtility> appUtility;
    private Mock<ISquareApiDataAccess> squareApiDAL;

    [SetUp]
    public void Setup()
    {
        logger = new Mock<ILogger<GetItemByItemId>>();
        squareUtility = new Mock<ISquareUtility>();
        squareDAL = new Mock<ISquareSdkDataAccess>();
        squareApiDAL = new Mock<ISquareApiDataAccess>();
        appUtility = new Mock<IApplicationUtility>();
    }

    [Test]
    [Parallelizable]
    [TestCaseSource(nameof(GetItemByItemIdCorrectResponseTestCases))]
    public async Task GetItemByItemId_CorrectResponseTest(CatalogObject? squareResponseMObject, CatalogInformation? requestBody, IActionResult expected)
    {
        // Arrange
        CatalogObject? populatedResponseBody = new CatalogObject("ITEM", Guid.NewGuid().ToString());
        RetrieveCatalogObjectResponse squareResponse = new(mObject: squareResponseMObject);
        appUtility.Setup(utility => utility.DeserializeStream<CatalogInformation>(It.IsAny<Stream>())).ReturnsAsync(requestBody);
        squareUtility.Setup(utility => utility.MapItemFromCatalogObjectResponse(It.IsAny<RetrieveCatalogObjectResponse?>())).Returns(new SquareItem("", "", "", ""));
        squareDAL.Setup(dal => dal.GetCatalogObjectById(It.IsAny<CatalogInformation>())).ReturnsAsync(squareResponse);
        Mock<HttpRequest> mockRequest = new();

        GetItemByItemId function = new GetItemByItemId(logger.Object, squareUtility.Object, squareDAL.Object, squareApiDAL.Object);

        // Act
        IActionResult actual = await function.Run(mockRequest.Object);

        // Assert
        Assert.That(actual.GetType(), Is.EqualTo(expected.GetType()));

    }

    public static IEnumerable<TestCaseData> GetItemByItemIdCorrectResponseTestCases()
    {
        CatalogInformation? goodRequestId = new ("GoodId");
        CatalogInformation? nullRequestId = null;

        CatalogObject? populatedResponseBody = new CatalogObject ("ITEM", "itemId", itemData: new CatalogItem());
        CatalogObject? emptyResponseBody = null;

        BadRequestResult badRequestResult = new ();
        NotFoundResult notFoundResult = new ();
        OkObjectResult okObjectResult = new (new List<SquareCategory>());

        yield return new TestCaseData(emptyResponseBody, goodRequestId, notFoundResult);
        yield return new TestCaseData(emptyResponseBody, nullRequestId, badRequestResult);
        yield return new TestCaseData(populatedResponseBody, goodRequestId, okObjectResult);
    }
}
