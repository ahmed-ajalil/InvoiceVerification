using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CR_CoreBot_DTO
{
    public class ShowCustomerInfoDTO
    {
        public int CustomerId { get; set; }

        public string? UserName { get; set; }

        public string? Password { get; set; }

        public string? Name { get; set; }

        public string? OrganizationName { get; set; }

        public string? OrganizationLogo { get; set; }

        public string? DatabaseName { get; set; }

        public string? BlobContainerName { get; set; }

        public string? AccountKey { get; set; }

        public string? AccountName { get; set; }

        public string? Category { get; set; }

        public DateTime? DateTime { get; set; }

        public int? RoleId { get; set; }

        public string? LoginWith { get; set; }
        public string? RoleType { get; set; }

    }
}
