using System;
using System.ComponentModel.DataAnnotations;

namespace BPX.Domain.DbModels
{
    public partial class Permit
    {
        public Permit()
        {
            //UserPermits = new HashSet<UserPermit>();
        }

        [Key]
        public int PermitID { get; set; }

        [Required]
        [StringLength(32)]
        public string PermitArea { get; set; }

        [Required]
        [StringLength(32)]
        public string PermitController { get; set; }

        [Required]
        [StringLength(32)]
        public string PermitName { get; set; }

        [Required]
        [StringLength(64)]
        public string PermitEnum { get; set; }

        [Required]
        [StringLength(1)]
        public string StatusFlag { get; set; }

        [Required]
        public int ModifiedBy { get; set; }

        [Required]
        public DateTime ModifiedDate { get; set; }

        //public virtual ICollection<UserPermit> UserPermits { get; set; }
    }
}