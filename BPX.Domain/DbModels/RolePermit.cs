using System;
using System.ComponentModel.DataAnnotations;

namespace BPX.Domain.DbModels
{
    public partial class RolePermit
    {
        [Key]
        public int RolePermitId { get; set; }

        [Required]
        public int RoleId { get; set; }

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

        //public virtual User User { get; set; }
    }
}