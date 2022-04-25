using System;
using System.ComponentModel.DataAnnotations;

namespace BPX.Domain.DbModels
{
    public class Role
    {
        public Role()
        {
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
    }
}