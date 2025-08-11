using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CR_CoreBot_DTO
{
    
        public class SessionTrackerModel
        {
            public int SessionTrackerId { get; set; }
            public TimeSpan SessionDuration { get; set; }
            public string Username { get; set; }
            public string LoginWith { get; set; }
            public DateTime LoginTime { get; set; }
            public int RoleId { get; set; }
            public string RoleName { get; set; }
            public int? TotalPrompt { get; set; }
            public int? TotalToken { get; set; }
            public DateTime CreatedDateTime { get; set; }
           
        }
       

    
}
