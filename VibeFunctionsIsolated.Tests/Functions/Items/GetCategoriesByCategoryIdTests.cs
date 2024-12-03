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
    public async Task GetCategoriesByCategoryId_GetsBadId_ReturnsNotFound()
    {
        // Arrange
        Mock<HttpRequest> mockRequest = new Mock<HttpRequest>();
        //mockRequest.Object.Body = Stream.Null;
        //mockRequest.Setup(x => x.Body).Returns(Stream.Null);
        CategoryId badId = new CategoryId("BadId");
        SearchCatalogObjectsResponse? squareResponse = new SearchCatalogObjectsResponse(objects: new List<CatalogObject>());

        squareDAL.Setup(dal => dal.SearchCategoryObjectsByParentId(It.IsAny<CategoryId>()).Result).Returns(squareResponse);
        squareUtility.Setup(x => x.DeserializeStream<CategoryId>(It.IsAny<Stream>()).Result).Returns(badId);

        GetCategoriesByCategoryId azureTestFunction = new GetCategoriesByCategoryId(logger.Object, squareDAL.Object, squareUtility.Object);


        // Act
        IActionResult result = await azureTestFunction.Run(mockRequest.Object);

        // Assert
        Assert.That(result.GetType(), Is.EqualTo(new NotFoundResult().GetType()));
    }
    IEnumerable<TestCaseData> GetCategoriesByCategoryIdTestCases()
    {


        yield return new TestCaseData();
    }

}
