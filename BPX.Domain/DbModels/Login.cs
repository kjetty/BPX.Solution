using System;
using System.ComponentModel.DataAnnotations;

namespace BPX.Domain.DbModels
{
    public partial class Login
    {
        [Key]
        public string LoginId { get; set; }

        [Required]
        [StringLength(32)]
        public string PasswordSalt { get; set; }

        [Required]
        [StringLength(128)]
        public string PasswordHash { get; set; }

        [Required]
        public int UserId { get; set; }


        [StringLength(128)]
        public string LoginToken { get; set; }

        public DateTime? LastLoginDate { get; set; }

        [Required]
        [StringLength(1)]
        public string StatusFlag { get; set; }

        [Required]
        public int ModifiedBy { get; set; }

        [Required]
        public DateTime ModifiedDate { get; set; }
    }
}