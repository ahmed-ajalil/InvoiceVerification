using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CR_HRPortalAI_DataAcess.CR_HRPortal
{
    public partial class TrainModelDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "*The Question field is required.")]
        public string Question { get; set; }

        [Required(ErrorMessage = "*The SQLQuery field is required.")]
        public string SQLQuery { get; set; }

    }
}
