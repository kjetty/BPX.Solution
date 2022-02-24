using System;
using System.ComponentModel.DataAnnotations;

namespace BPX.Domain.DbModels
{
    public partial class Login
    {
        [Key]
        [StringLength(24)]
        public string LoginUUId { get; set; }

        [StringLength(32)]
        public string LoginName { get; set; }

        [StringLength(128)]
        public string PasswordHash { get; set; }

        [StringLength(16)]
        public string PIVId { get; set; }

        [Required]
        [StringLength(1)]
        public string LoginType { get; set; }

        [Required]
        [StringLength(40)]
        public string RToken { get; set; }

        [Required]
        public DateTime LastLoginDate { get; set; }

        [Required]
        [StringLength(1)]
        public string StatusFlag { get; set; }

        [Required]
        public int ModifiedBy { get; set; }

        [Required]
        public DateTime ModifiedDate { get; set; }
    }
}