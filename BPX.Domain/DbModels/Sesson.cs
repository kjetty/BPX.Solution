using System;
using System.ComponentModel.DataAnnotations;

namespace BPX.Domain.DbModels
{
    public class Sesson
    {
        public Sesson()
        {
        }

        [Key]
        [StringLength(24)]
        public string SessonUUId { get; set; }

        [Required]
        [StringLength(40)]
        public string SToken { get; set; }

        [Required]
        public DateTime LastAccessTime { get; set; }
    }
}