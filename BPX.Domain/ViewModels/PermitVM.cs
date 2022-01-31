using BPX.Domain.DbModels;
using System.ComponentModel.DataAnnotations;

namespace BPX.Domain.ViewModels
{
    public class PermitMiniViewModel
    {
        [Key]
        [Display(Name = "Permit ID")]
        public int PermitId { get; set; }

        [Required]
        [StringLength(32)]
        [Display(Name = "Area")]
        public string PermitArea { get; set; }

        [Required]
        [StringLength(32)]
        [Display(Name = "Controller")]
        public string PermitController { get; set; }

        [Required]
        [StringLength(32)]
        [Display(Name = "Name")]
        public string PermitName { get; set; }

        [Required]
        [StringLength(64)]
        [Display(Name = "Enumerator")]
        public string PermitEnum { get; set; }

        public static explicit operator PermitMiniViewModel(Permit dm)
        {
            if (dm == null)
                return new PermitMiniViewModel();

            return new PermitMiniViewModel
            {
                PermitId = dm.PermitId,
                PermitArea = dm.PermitArea,
                PermitController = dm.PermitController,
                PermitName = dm.PermitName,
                PermitEnum = dm.PermitEnum
            };
        }

        public static explicit operator Permit(PermitMiniViewModel vm)
        {
            if (vm == null)
                return new Permit();

            return new Permit
            {
                PermitId = vm.PermitId,
                PermitArea = vm.PermitArea,
                PermitController = vm.PermitController,
                PermitName = vm.PermitName,
                PermitEnum = vm.PermitEnum
            };
        }
    }

    public class PermitViewModel : PermitMiniViewModel
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

        public static explicit operator PermitViewModel(Permit dm)
        {
            if (dm == null)
                return new PermitViewModel();

            return new PermitViewModel
            {
                // set core data
                PermitId = dm.PermitId,
                PermitArea = dm.PermitArea,
                PermitController = dm.PermitController,
                PermitName = dm.PermitName,
                PermitEnum = dm.PermitEnum,
                // set generic data
                StatusFlag = dm.StatusFlag,
                ModifiedBy = dm.ModifiedBy,
                ModifiedDate = dm.ModifiedDate
            };
        }

        public static explicit operator Permit(PermitViewModel vm)
        {
            if (vm == null)
                return new Permit();

            return new Permit
            {
                // set core data
                PermitId = vm.PermitId,
                PermitArea = vm.PermitArea,
                PermitController = vm.PermitController,
                PermitName = vm.PermitName,
                PermitEnum = vm.PermitEnum,
                // set generic data
                StatusFlag = vm.StatusFlag,
                ModifiedBy = vm.ModifiedBy,
                ModifiedDate = vm.ModifiedDate
            };
        }
    }
}