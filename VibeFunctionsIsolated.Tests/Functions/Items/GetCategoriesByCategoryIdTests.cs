using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Square.Models;
using VibeCollectiveFunctions.Utility;
using VibeFunctionsIsolated.DAL;
using VibeFunctionsIsolated.Functions.Items;
using VibeFunctionsIsolated.Models;

namespace VibeFunctionsIsolated.Tests.Functions.Items;

[TestFixture]
[Parallelizable]
public class GetCategoriesByCategoryIdTests
{
    private Mock<ILogger<GetCategoriesByCategoryId>> logger;
    private Mock<ISquareUtility> squareUtility;
    private Mock<ISquareDAL> squareDAL;

    [SetUp]
    public void Setup()
    {
        logger = new Mock<ILogger<GetCategoriesByCategoryId>>();
        squareUtility = new Mock<ISquareUtility>();
        squareDAL = new Mock<ISquareDAL>();
    }

    [Test]
    [Parallelizable]
    [TestCaseSource(nameof(GetCategoriesByCategoryIdTestCases))]
    public async Task GetCategoriesByCategoryId_CorrectResponseTest(List<CatalogObject> squareResponseBody, CategoryId? requestBody, IActionResult expected)
    {
        // Arrange
        Mock<HttpRequest> mockRequest = new();
        CategoryId badId = new("BadId");
        SearchCatalogObjectsResponse? squareResponse = new(objects: squareResponseBody);

        squareDAL.Setup(dal => dal.SearchCategoryObjectsByParentId(It.IsAny<CategoryId>()).Result).Returns(squareResponse);
        squareUtility.Setup(x => x.DeserializeStream<CategoryId>(It.IsAny<Stream>()).Result).Returns(requestBody);

        GetCategoriesByCategoryId azureTestFunction = new(logger.Object, squareDAL.Object, squareUtility.Object);

        // Act
        IActionResult result = await azureTestFunction.Run(mockRequest.Object);

        // Assert
        Assert.That(result.GetType(), Is.EqualTo(expected.GetType()));
    }

    private static IEnumerable<TestCaseData> GetCategoriesByCategoryIdTestCases()
    {
        CategoryId? goodId = new("GoodId");
        CategoryId? nullId = null;

        List<CatalogObject> populatedResponseBody = [new CatalogObject("ITEM", Guid.NewGuid().ToString())];
        List<CatalogObject>? emptyResponseBody = new();

        BadRequestResult badRequestResult = new();
        NotFoundResult notFoundResult = new();
        OkObjectResult okObjectResult = new(new List<SquareCategory>());

        yield return new TestCaseData(emptyResponseBody, goodId, notFoundResult);
        yield return new TestCaseData(emptyResponseBody, nullId, badRequestResult);
        yield return new TestCaseData(populatedResponseBody, goodId, okObjectResult);
    }
}
