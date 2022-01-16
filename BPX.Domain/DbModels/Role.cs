using System;
using System.ComponentModel.DataAnnotations;

namespace BPX.Domain.DbModels
{
    public partial class Role
    {
        public Role()
        {
            //UserRoles = new HashSet<UserRole>();
        }

        [Key]
        public int RoleId { get; set; }

        [Required]
        [StringLength(32)]
        public string RoleName { get; set; }

        [StringLength(128)]
        public string RoleDescription { get; set; }

        [Required]
        [StringLength(1)]
        public string StatusFlag { get; set; }

        [Required]
        public int ModifiedBy { get; set; }

        [Required]
        public DateTime ModifiedDate { get; set; }

        //public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}