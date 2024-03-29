﻿using System;
using System.ComponentModel.DataAnnotations;

namespace BPX.Domain.DbModels
{
    public class Error
    {
        public Error()
        {
        }

        [Key]
        public int ErrorId { get; set; }

        [StringLength(8000)]
        public string ErrorData { get; set; }

        [Required]
        public DateTime ErrorDate { get; set; } = DateTime.Now;
    }
}
