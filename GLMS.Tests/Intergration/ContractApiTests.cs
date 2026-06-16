using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace GLMS.Tests.Integration
{
    public class ApiIntegrationTests
    {
        private readonly HttpClient _client;

        public ApiIntegrationTests()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7085/")
            };
        }

        // ==========================================
        // CONTRACT TESTS
        // ==========================================

        [Fact]
        public async Task GetContracts_Returns200OK()
        {
            var response = await _client.GetAsync("api/contracts");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetContracts_ReturnsData()
        {
            var contracts =
                await _client.GetFromJsonAsync<object>("api/contracts");

            contracts.Should().NotBeNull();
        }

        [Fact]
        public async Task GetContractById_ReturnsSuccess()
        {
            var response =
                await _client.GetAsync("api/contracts/1");

            response.IsSuccessStatusCode.Should().BeTrue();
        }

        // ==========================================
        // CLIENT TESTS
        // ==========================================

        [Fact]
        public async Task GetClients_Returns200OK()
        {
            var response =
                await _client.GetAsync("api/clients");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetClients_ReturnsData()
        {
            var clients =
                await _client.GetFromJsonAsync<object>("api/clients");

            clients.Should().NotBeNull();
        }

        // ==========================================
        // SERVICE REQUEST TESTS
        // ==========================================

        [Fact]
        public async Task GetServiceRequests_Returns200OK()
        {
            var response =
                await _client.GetAsync("api/servicerequests");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetServiceRequests_ReturnsData()
        {
            var requests =
                await _client.GetFromJsonAsync<object>(
                    "api/servicerequests");

            requests.Should().NotBeNull();
        }
    }
}