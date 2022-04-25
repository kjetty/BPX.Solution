using System;
using System.ComponentModel.DataAnnotations;

namespace BPX.Domain.DbModels
{
    public class Portal
    {
        public Portal()
        {
        }

        [Key]
        [StringLength(24)]
        public string PortalUUId { get; set; }

        [Required]
        [StringLength(40)]
        public string PToken { get; set; }

        [Required]
        public DateTime LastAccessTime { get; set; }
    }
}