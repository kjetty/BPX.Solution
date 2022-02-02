using BPX.Domain.DbModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace BPX.Domain.ViewModels
{
    public class LoginMiniViewModel
    {
        [Key]
        [Display(Name = "Login Name")]
        public string LoginId { get; set; }

        [Required]
        [StringLength(128)]
        [Display(Name = "Password")]
        public string PasswordHash { get; set; }

        [Required]
        public int UserId { get; set; }

        [StringLength(128)]
        public string LoginToken { get; set; }

        public DateTime LastLoginDate { get; set; }

        public static explicit operator LoginMiniViewModel(Login dm)
        {
            if (dm == null)
                return new LoginViewModel();

            return new LoginViewModel
            {
                // set core data
                LoginId = dm.LoginId,
                PasswordHash = dm.PasswordHash,
                UserId = dm.UserId,
                LoginToken = dm.LoginToken,
                LastLoginDate = (DateTime) dm.LastLoginDate                
            };
        }

        public static explicit operator Login(LoginMiniViewModel vm)
        {
            if (vm == null)
                return new Login();

            return new Login
            {
                // set core data
                LoginId = vm.LoginId,
                PasswordHash = vm.PasswordHash,
                LoginToken = vm.LoginToken,
                LastLoginDate = vm.LastLoginDate,
                UserId = vm.UserId,
            };
        }
    }

    public class LoginViewModel : LoginMiniViewModel
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

        public static explicit operator LoginViewModel(Login dm)
        {
            if (dm == null)
                return new LoginViewModel();

            return new LoginViewModel
            {
                // set core data
                LoginId = dm.LoginId,
                PasswordHash = dm.PasswordHash,
                UserId = dm.UserId,
                LoginToken = dm.LoginToken,
                LastLoginDate = (DateTime) dm.LastLoginDate,
                // set generic data
                StatusFlag = dm.StatusFlag,
                ModifiedBy = dm.ModifiedBy,
                ModifiedDate = dm.ModifiedDate
            };
        }

        public static explicit operator Login(LoginViewModel vm)
        {
            if (vm == null)
                return new Login();

            return new Login
            {
                // set core data
                LoginId = vm.LoginId,
                PasswordHash = vm.PasswordHash,
                UserId = vm.UserId,
                LoginToken = vm.LoginToken,
                LastLoginDate = vm.LastLoginDate,
                // set generic data
                StatusFlag = vm.StatusFlag,
                ModifiedBy = vm.ModifiedBy,
                ModifiedDate = vm.ModifiedDate
            };
        }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

	public class BPXLoginViewModel
	{
		[Required]
		[Display(Name = "Login Id")]
		public string LoginId { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[Display(Name = "Return URL")]
		public string ReturnUrl { get; set; }
	}

	public class RegisterViewModel
    {
        [Required]
        [StringLength(32)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(32)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(64)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Cell Phone")]
        public string CellPhone { get; set; }

        [Required]
        [Display(Name = "Login Id")]
        public string LoginId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string redirectAction { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Key]
        [Display(Name = "User Id")]
        public int UserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}