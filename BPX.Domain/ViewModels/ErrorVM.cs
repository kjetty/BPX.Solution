using System;
using System.ComponentModel.DataAnnotations;

namespace BPX.Domain.ViewModels
{
    public class ErrorViewModel
    {
        [Key]
        [Display(Name = "Error Id")]
        public int ErrorId { get; set; }

        [StringLength(8000)]
        [Display(Name = "Error Data")]
        public string ErrorData { get; set; }

        [Required]
        [Display(Name = "Error Date")]
        public DateTime ErrorDate { get; set; }
    }
}