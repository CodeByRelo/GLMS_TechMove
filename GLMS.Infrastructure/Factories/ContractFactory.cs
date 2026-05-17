using GLMS.Core.Entities;
using GLMS.Core.Enums;
using GLMS.Core.Interfaces;
using System;

namespace GLMS.Infrastructure.Factories
{
    public class ContractFactory : IContractFactory
    {
        public Contract CreateContract(string serviceLevel)
        {
            return new Contract
            {
                ServiceLevel = serviceLevel,
                StartDate = DateTime.Now,
                Status = ContractStatus.Draft
            };
        }
    }
}