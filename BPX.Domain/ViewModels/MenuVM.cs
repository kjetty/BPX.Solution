using BPX.Domain.DbModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace BPX.Domain.ViewModels
{
    public class MenuMiniViewModel
    {
        [Key]
        [Display(Name = "Menu Id")]
        public int MenuId { get; set; }

        [Required]
        [StringLength(32)]
        [Display(Name = "Menu Name")]
        public string MenuName { get; set; }

        [StringLength(128)]
        [Display(Name = "Menu Description")]
        public string MenuDescription { get; set; }

        [Required]
        [StringLength(1024)]
        [Display(Name = "Menu URL")]
        public string MenuURL { get; set; }

        [Display(Name = "Parent Menu Id")]
        public int ParentMenuId { get; set; }

        [Display(Name = "Hierarchy Level")]
        public int HLevel { get; set; }

        [Display(Name = "Order Number")]
        public int OrderNumber { get; set; }

        [StringLength(32)]
        [Display(Name = "Tree Path")]
        public string TreePath { get; set; }

        public static explicit operator MenuMiniViewModel(Menu dm)
        {
            if (dm == null)
                return new MenuMiniViewModel();

            return new MenuMiniViewModel
            {
                // set core data
                MenuId = dm.MenuId,
                MenuName = dm.MenuName,
                MenuDescription = dm.MenuDescription,
                MenuURL = dm.MenuURL,
                ParentMenuId = dm.ParentMenuId,
                HLevel = dm.HLevel,
                OrderNumber = dm.OrderNumber,
                TreePath = dm.TreePath
            };
        }

        public static explicit operator Menu(MenuMiniViewModel vm)
        {
            if (vm == null)
                return new Menu();

            return new Menu
            {
                // set core data
                MenuId = vm.MenuId,
                MenuName = vm.MenuName,
                MenuDescription = vm.MenuDescription,
                MenuURL = vm.MenuURL,
                ParentMenuId = vm.ParentMenuId,
                HLevel = vm.HLevel,
                OrderNumber = vm.OrderNumber,
                TreePath = vm.TreePath
            };
        }
    }

    public class MenuViewModel : MenuMiniViewModel
    {
        [Required]
        [StringLength(1)]
        [Display(Name = "Status Flag")]
        public string StatusFlag { get; set; }

        [Required]
        [Display(Name = "Modified By")]
        public int ModifiedBy { get; set; }

        [Required]
        [Display(Name = "Modified On")]
        public DateTime ModifiedDate { get; set; }

        public static explicit operator MenuViewModel(Menu dm)
        {
            if (dm == null)
                return new MenuViewModel();

            return new MenuViewModel
            {
                // set core data
                MenuId = dm.MenuId,
                MenuName = dm.MenuName,
                MenuDescription = dm.MenuDescription,
                MenuURL = dm.MenuURL,
                ParentMenuId = dm.ParentMenuId,
                HLevel = dm.HLevel,
                OrderNumber = dm.OrderNumber,
                TreePath = dm.TreePath,
                // set generic data
                StatusFlag = dm.StatusFlag,
                ModifiedBy = dm.ModifiedBy,
                ModifiedDate = dm.ModifiedDate
            };
        }

        public static explicit operator Menu(MenuViewModel vm)
        {
            if (vm == null)
                return new Menu();

            return new Menu
            {
                // set core data
                MenuId = vm.MenuId,
                MenuName = vm.MenuName,
                MenuDescription = vm.MenuDescription,
                MenuURL = vm.MenuURL,
                ParentMenuId = vm.ParentMenuId,
                HLevel = vm.HLevel,
                OrderNumber = vm.OrderNumber,
                TreePath = vm.TreePath,
                // set generic data
                StatusFlag = vm.StatusFlag,
                ModifiedBy = vm.ModifiedBy,
                ModifiedDate = vm.ModifiedDate
            };
        }
    }
}