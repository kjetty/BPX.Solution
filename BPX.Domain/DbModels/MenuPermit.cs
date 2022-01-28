using System;
using System.ComponentModel.DataAnnotations;

namespace BPX.Domain.DbModels
{
    public partial class MenuPermit
    {
        [Key]
        public int MenuPermitId { get; set; }

        [Required]
        public int MenuId { get; set; }

        [Required]
        public int PermitId { get; set; }

        [Required]
        [StringLength(1)]
        public string StatusFlag { get; set; }

        [Required]
        public int ModifiedBy { get; set; }

        [Required]
        public DateTime ModifiedDate { get; set; }

        //public virtual Role Role { get; set; }

        //public virtual Menu Menu { get; set; }
    }
}