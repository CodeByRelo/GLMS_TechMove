using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;

namespace GLMS.Core.Entities
{
    public class Client
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string ContactDetails { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;

        // Navigation Property
        public List<Contract> Contracts { get; set; } = new();
    }
}