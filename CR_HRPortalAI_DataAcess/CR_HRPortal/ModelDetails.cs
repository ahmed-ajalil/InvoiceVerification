using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CR_HRPortalAI_DataAcess.CR_HRPortal
{
    public partial class ModelDetails
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "*The EndPoint field is required.")]
        public string EndPoint { get; set; }
        [Required(ErrorMessage = "*The ApiKey field is required.")]
        public string ApiKey { get; set; }
        [Required(ErrorMessage = "*The DeploymentName field is required.")]
        public string DeploymentName { get; set; }
        [Required(ErrorMessage = "*The ApiVersion field is required.")]
        public string ApiVersion { get; set; }
         
        public string ModelName { get; set; }
         
        public string Size { get; set; }

        public int Status { get; set; }
        public int DefaultType { get; set; }

    }

}
