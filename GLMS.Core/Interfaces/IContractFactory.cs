using GLMS.Core.Entities;

namespace GLMS.Core.Interfaces
{
    public interface IContractFactory
    {
        Contract CreateContract(string serviceLevel);
    }
}