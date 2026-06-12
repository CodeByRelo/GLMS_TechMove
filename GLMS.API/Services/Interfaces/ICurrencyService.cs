using System.Threading.Tasks;

namespace GLMS.API.Services.Interfaces
{
    public interface ICurrencyService
    {
        Task<decimal> ConvertUsdToZar(decimal usdAmount);
    }
}