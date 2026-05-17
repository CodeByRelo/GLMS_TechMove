using Xunit;
using System;

namespace GLMS.Tests.Services
{
    // Minimal CurrencyService stub for testing
    public class CurrencyService
    {
        public decimal ConvertUsdToZar(decimal usd, decimal rate)
        {
            if (rate <= 0) throw new ArgumentException("Rate must be greater than zero.");
            return usd * rate;
        }
    }

    public class CurrencyServiceTests
    {
        private readonly CurrencyService _currencyService;

        public CurrencyServiceTests()
        {
            _currencyService = new CurrencyService();
        }

        [Theory]
        [InlineData(100, 18.5, 1850)]
        [InlineData(250, 18.5, 4625)]
        public void ConvertUsdToZar_ReturnsCorrectValue(decimal usd, decimal rate, decimal expected)
        {
            var result = _currencyService.ConvertUsdToZar(usd, rate);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ConvertUsdToZar_ThrowsException_WhenRateIsZero()
        {
            Assert.Throws<ArgumentException>(() => _currencyService.ConvertUsdToZar(100, 0));
        }
    }
}
