using System;
using System.ComponentModel.DataAnnotations;

namespace BPX.Domain.DbModels
{
    public partial class MenuRole
    {
        [Key]
        public int MenuRoleId { get; set; }

        [Required]
        public int MenuId { get; set; }

        [Required]
        public int RoleId { get; set; }

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