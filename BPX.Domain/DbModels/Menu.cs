using System;
using System.ComponentModel.DataAnnotations;

namespace BPX.Domain.DbModels
{
    public class Menu
    {
        public Menu()
        {
        }

        [Key]
        public int MenuId { get; set; }

        [Required]
        [StringLength(32)]
        public string MenuName { get; set; }

        [StringLength(128)]
        public string MenuDescription { get; set; }

        [Required]
        [StringLength(1024)]
        public string MenuURL { get; set; }

        public int ParentMenuId { get; set; }

        public int HLevel { get; set; }

        public int OrderNumber { get; set; }

        [StringLength(32)]
        public string TreePath { get; set; }

        [Required]
        [StringLength(1)]
        public string StatusFlag { get; set; }

        [Required]
        public int ModifiedBy { get; set; }

        [Required]
        public DateTime ModifiedDate { get; set; }
    }
}