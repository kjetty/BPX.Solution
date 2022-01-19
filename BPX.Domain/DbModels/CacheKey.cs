using System;
using System.ComponentModel.DataAnnotations;

namespace BPX.Domain.DbModels
{
    public partial class CacheKey
    {
        public CacheKey()
        {
        }

        [Key]
        [StringLength(48)]
        public string CacheKeyName { get; set; }


        [Required]
        public DateTime ModifiedDate { get; set; }
    }
}