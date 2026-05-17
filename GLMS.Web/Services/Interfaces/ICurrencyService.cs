using System.Threading.Tasks;

namespace GLMS.Web.Services.Interfaces
{
    public interface ICurrencyService
    {
        Task<decimal> ConvertUsdToZar(decimal usdAmount);
    }
}