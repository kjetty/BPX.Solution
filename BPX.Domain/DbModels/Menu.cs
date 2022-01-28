using System;
using System.ComponentModel.DataAnnotations;

namespace BPX.Domain.DbModels
{
    public partial class Menu
    {
        public Menu()
        {
            //MenuPermits = new HashSet<MenuPermit>();
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

        [Required]
        [StringLength(1)]
        public string StatusFlag { get; set; }

        [Required]
        public int ModifiedBy { get; set; }

        [Required]
        public DateTime ModifiedDate { get; set; }

        //public virtual ICollection<MenuPermit> MenuPermits { get; set; }
    }
}