using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using VibeFunctionsIsolated.Functions.TeamMembers;
using VibeFunctionsIsolated.Models.Square;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Tests.Functions.TeamMember
{
    [TestFixture]
    [Parallelizable(ParallelScope.Self)]
    public class GetAllTeamMembersTests
    {
        private Mock<ILogger<GetAllTeamMembers>> mockLogger;
        private Mock<ISquareUtility> mockSquareUtility;

        [SetUp]
        public void Setup()
        {
            mockLogger = new Mock<ILogger<GetAllTeamMembers>>();
            mockSquareUtility = new Mock<ISquareUtility>();
        }

        [Test]
        [TestCaseSource(nameof(GetAllTeamMembersTestCases))]
        [Parallelizable(ParallelScope.Self)]
        public async Task GetAllTeamMembersUnitTests(List<SquareTeamMember> returnedTeamMembers, IActionResult expected)
        {
            // Arrange
            mockSquareUtility.Setup(x => x.MapAllBookableTeamMembers()).ReturnsAsync(returnedTeamMembers);
            GetAllTeamMembers function = new GetAllTeamMembers(mockLogger.Object, mockSquareUtility.Object);
            Mock<HttpRequest> mockRequest = new();

            // Act
            IActionResult actual = await function.Run(mockRequest.Object);

            // Assert
            Assert.That(actual.GetType(), Is.EqualTo(expected.GetType()));
        }

        private static IEnumerable<TestCaseData> GetAllTeamMembersTestCases()
        {
            List<SquareTeamMember> emptyTeamMemberList = new List<SquareTeamMember>();
            List<SquareTeamMember> populatedTeamMemberList = new List<SquareTeamMember>
            {
                new SquareTeamMember("1", "Test Name", "Test Description", "Test Image URL")
            };  

            IActionResult notFoundResult = new NotFoundResult();
            IActionResult okResult = new OkObjectResult(populatedTeamMemberList);

            yield return new TestCaseData(emptyTeamMemberList, notFoundResult);
            yield return new TestCaseData(populatedTeamMemberList, okResult);
        }
    }
}
