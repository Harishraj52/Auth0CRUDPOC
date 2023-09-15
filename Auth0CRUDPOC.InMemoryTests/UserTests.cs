namespace Auth0CRUDPOC.InMemoryTests
{
    using System.Net;
    using System.Net.Http;
    using Newtonsoft.Json;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using WireMock.Server;
    public class UserTests
    {
        private readonly WireMockServer _wireMockServer;
        private readonly HttpClient _httpClient;
        public UserTests()
        {
            // Start the WireMock server
            _wireMockServer = WireMockServer.Start("http://localhost:5067");
            // Create an instance of TestServer using the WireMock server's URL
            // Create an instance of HttpClient to make requests to the TestServer
            _httpClient = new HttpClient();
        }
        [Fact]
        public async Task CreateUser_ValidUser_ReturnsSuccess()
        {
            // Arrange
            var userId = "user123";
            var requestBody = new { Id = userId, Name = "John Doe" };
            _wireMockServer.Given(Request.Create().WithPath("/api/users").UsingPost())
                .RespondWith(Response.Create().WithStatusCode(200));
            // Act
            var response = await _httpClient.PostAsJsonAsync($"{_wireMockServer.Urls[0]}/api/users", requestBody);
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        // Additional tests for updating and deleting users can be added here
        public void Dispose()
        {
            // Dispose the TestServer and WireMockServer
            _wireMockServer.Stop();
        }
    }
}