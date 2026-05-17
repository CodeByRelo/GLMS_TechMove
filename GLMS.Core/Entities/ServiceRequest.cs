using System;
using System.ComponentModel.DataAnnotations;

namespace GLMS.Core.Entities
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cost in USD is required.")]
        public decimal CostUSD { get; set; }

        public decimal Cost { get; set; }
        public decimal CostZAR { get; set; }

        public string Status { get; set; } = "Pending";
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Foreign Key
        public int ContractId { get; set; }

        // Navigation
        public Contract? Contract { get; set; }
    }
}
