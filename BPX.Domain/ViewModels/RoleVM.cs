using BPX.Domain.DbModels;
using System.ComponentModel.DataAnnotations;

namespace BPX.Domain.ViewModels
{
    public class RoleMiniViewModel
    {
        [Key]
        [Display(Name = "Role ID")]
        public int RoleId { get; set; }

        [Required]
        [StringLength(32)]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        [StringLength(128)]
        [Display(Name = "Role Description")]
        public string RoleDescription { get; set; }


        public static explicit operator RoleMiniViewModel(Role dm)
        {
            if (dm == null)
                return new RoleMiniViewModel();

            return new RoleMiniViewModel
            {
                // set core data
                RoleId = dm.RoleId,
                RoleName = dm.RoleName,
                RoleDescription = dm.RoleDescription
            };
        }

        public static explicit operator Role(RoleMiniViewModel vm)
        {
            if (vm == null)
                return new Role();

            return new Role
            {
                RoleId = vm.RoleId,
                RoleName = vm.RoleName,
                RoleDescription = vm.RoleDescription
            };
        }
    }

    public class RoleViewModel : RoleMiniViewModel
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
        public System.DateTime ModifiedDate { get; set; }

        public static explicit operator RoleViewModel(Role dm)
        {
            if (dm == null)
                return new RoleViewModel();

            return new RoleViewModel
            {
                // set core data
                RoleId = dm.RoleId,
                RoleName = dm.RoleName,
                RoleDescription = dm.RoleDescription,
                // set generic data
                StatusFlag = dm.StatusFlag,
                ModifiedBy = dm.ModifiedBy,
                ModifiedDate = dm.ModifiedDate
            };
        }

        public static explicit operator Role(RoleViewModel vm)
        {
            if (vm == null)
                return new Role();

            return new Role
            {
                // set core data
                RoleId = vm.RoleId,
                RoleName = vm.RoleName,
                RoleDescription = vm.RoleDescription,
                // set generic data
                StatusFlag = vm.StatusFlag,
                ModifiedBy = vm.ModifiedBy,
                ModifiedDate = vm.ModifiedDate
            };
        }
    }
}