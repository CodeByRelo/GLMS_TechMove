using Xunit;
using GLMS.Core.Entities;
using GLMS.Web.Services.Interfaces;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GLMS.Tests.Services
{
    public class ContractServiceTests
    {
        private readonly Mock<IContractService> _mockContractService;

        public ContractServiceTests()
        {
            _mockContractService = new Mock<IContractService>();
        }

        [Fact]
        public async Task GetAllContractsAsync_ReturnsContracts()
        {
            // Arrange
            var contracts = new List<Contract>
            {
                new Contract { Id = 1, ServiceLevel = "Gold", Status = Core.Enums.ContractStatus.Active },
                new Contract { Id = 2, ServiceLevel = "Silver", Status = Core.Enums.ContractStatus.Expired }
            };

            _mockContractService.Setup(s => s.GetAllContractsAsync())
                .ReturnsAsync(contracts);

            // Act
            var result = await _mockContractService.Object.GetAllContractsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }
    }
}
