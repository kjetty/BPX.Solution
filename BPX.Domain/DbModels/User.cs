﻿using System;
using System.ComponentModel.DataAnnotations;
//using System.Web.Mvc;

namespace BPX.Domain.DbModels
{
    public class User
    {
        public User()
        {
        }

        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(32)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(32)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(64)]
        public string Email { get; set; }

        [StringLength(16)]
        public string Mobile { get; set; }

        [Required]
        [StringLength(24)]
        public string UserUUId { get; set; }

        [Required]
        [StringLength(24)]
        public string LoginUUId { get; set; }

        [Required]
        [StringLength(24)]
        public string SessonUUId { get; set; }

        [Required]
        [StringLength(1)]
        public string StatusFlag { get; set; }

        [Required]
        public int ModifiedBy { get; set; }

        [Required]
        public DateTime ModifiedDate { get; set; }
    }
}