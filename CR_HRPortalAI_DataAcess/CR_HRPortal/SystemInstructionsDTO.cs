using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CR_HRPortalAI_DataAcess.CR_HRPortal
{
    public partial class SystemInstructionsDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "*The Rules field is required.")]
        public string Rules { get; set; }
        [Required(ErrorMessage = "*The Industry field is required.")]
        public string Industry { get; set; }

        public int Status { get; set; }

        public int DefaultType { get; set; }
    }
}
