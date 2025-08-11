using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CR_CoreBot_DTO
{
    public class AdLoginInfoDTO
    {
        public int? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public string? EmailId { get; set; }
        public string? DatabaseName { get; set; }
        public string? Category { get; set; }
        public string? OrganizationName { get; set; }
        public string? Logo { get; set; }
        public string? BlobContainerName { get; set; }
        public int? RoleId { get; set; }
        public string UserName { get; set; }
        public int CustomerId { get; set; }
        public string Password { get; set; }
        public int? UserId { get; set; }
        public string Role { get; set; }
        public int AdLoggedInUserId { get; set; }
    }
}
