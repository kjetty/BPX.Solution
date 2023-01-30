using BPX.Domain.DbModels;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace BPX.Domain.ViewModels
{
    public class UserMiniViewModel
    {
        [Key]
        [Display(Name = "User Id")]
        public int UserId { get; set; }

        [Required]
        [StringLength(32)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(32)]
        //[MinLength(8, ErrorMessage = "Minimum input length is 8 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Remote(action: "RemoteVerifyEmail", controller: "User", areaName: "Identity", ErrorMessage = "remote email is not valid")]
        [Required]
        [EmailAddress]
        [StringLength(64)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(16)]
        [Display(Name = "Mobile")]
        public string Mobile { get; set; }

        //[Required]
        //[StringLength(24)]
        //[Display(Name = "UserUUId")]
        //public string UserUUId { get; set; }

        //[Required]
        //[StringLength(24)]
        //[Display(Name = "LoginUUId")]
        //public string LoginUUId { get; set; }

        //[Required]
        //[StringLength(24)]
        //[Display(Name = "SessonUUId")]
        //public string SessonUUId { get; set; }


		//  an explicit conversion involves casting from one type to another.
        public static explicit operator UserMiniViewModel(User dm)
        {
            if (dm == null)
                return new UserMiniViewModel();

            return new UserMiniViewModel
            {
                // set core data
                UserId = dm.UserId,
                FirstName = dm.FirstName,
                LastName = dm.LastName,
                Email = dm.Email,
                Mobile = dm.Mobile
                //UserUUId = dm.UserUUId,
                //LoginUUId = dm.LoginUUId,
                //SessonUUId = dm.SessonUUId
            };
        }

        public static explicit operator User(UserMiniViewModel vm)
        {
            if (vm == null)
                return new User();

            return new User
            {
                // set core data
                UserId = vm.UserId,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Email = vm.Email,
                Mobile = vm.Mobile
                //UserUUId = vm.UserUUId,
                //LoginUUId = vm.LoginUUId,
                //SessonUUId = vm.SessonUUId
            };
        }
    }

    public class UserViewModel : UserMiniViewModel
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

        public static explicit operator UserViewModel(User dm)
        {
            if (dm == null)
                return new UserViewModel();

            return new UserViewModel
            {
                // set core data
                UserId = dm.UserId,
                FirstName = dm.FirstName,
                LastName = dm.LastName,
                Email = dm.Email,
                Mobile = dm.Mobile,
                //UserUUId = dm.UserUUId,
                //LoginUUId = dm.LoginUUId,
                //SessonUUId = dm.SessonUUId,
                // set generic data
                StatusFlag = dm.StatusFlag,
                ModifiedBy = dm.ModifiedBy,
                ModifiedDate = dm.ModifiedDate
            };
        }

        public static explicit operator User(UserViewModel vm)
        {
            if (vm == null)
                return new User();

            return new User
            {
                // set core data
                UserId = vm.UserId,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Email = vm.Email,
                Mobile = vm.Mobile,
                //UserUUId = vm.UserUUId,
                //LoginUUId = vm.LoginUUId,
                //SessonUUId = vm.SessonUUId,
                // set generic data
                StatusFlag = vm.StatusFlag,
                ModifiedBy = vm.ModifiedBy,
                ModifiedDate = vm.ModifiedDate
            };
        }
    }
}