using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CR_CoreBot_DTO
{
    public class LoggedInCustomerDetailsDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string EmailId { get; set; }
        public string DatabaseName { get; set; }

        public string Category { get; set; }

        public string OrganizationName { get; set; }
        public string Logo { get; set; }
        public int RoleId { get; set; }
        public int CustomerId { get; set; }
        public int? UserId { get; set; }
    }
}
