using System;
using System.ComponentModel.DataAnnotations;

namespace BPX.Domain.DbModels
{
    public partial class MemoryCacheKey
    {
        public MemoryCacheKey()
        {
        }

        [Key]
        [StringLength(48)]
        public string CacheKey { get; set; }


        [Required]
        public DateTime ModifiedDate { get; set; }
    }
}