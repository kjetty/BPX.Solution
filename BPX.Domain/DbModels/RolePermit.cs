using System;
using System.ComponentModel.DataAnnotations;

namespace BPX.Domain.DbModels
{
    public class RolePermit
    {
        public RolePermit()
        {
        }

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
    }
}