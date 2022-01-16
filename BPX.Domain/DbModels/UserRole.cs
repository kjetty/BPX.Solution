using System;
using System.ComponentModel.DataAnnotations;

namespace BPX.Domain.DbModels
{
    public partial class UserRole
    {
        [Key]
        public int UserRoleId { get; set; }

        [Required]
        public int UserId { get; set; }

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

        //public virtual User User { get; set; }
    }
}