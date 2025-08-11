using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CR_HRPortalAI_DataAcess.Models
{
    public class ConnectionModel
    {
            public string DefaultConnection { get; set; }
           public string RestoreDefaultConnection { get; set; }
        
            public string DatabaseName { get; set; }
            public string Connection { get; set; }
    }
}
