using System.ComponentModel.DataAnnotations;

namespace BPX.Domain.ViewModels
{
    public class ErrorViewModel
    {
        [Key]
        [Display(Name = "Error Id")]
        public int ErrorId { get; set; }

        [StringLength(1000)]
        [Display(Name = "Error Data")]
        public int ErrorData { get; set; }

        [Required]
        [Display(Name = "Error Date")]
        public int ErrorDate { get; set; }
    }
}