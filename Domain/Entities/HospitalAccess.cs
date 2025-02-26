using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class HospitalAccess : BaseModel
    {
        [Key]
        public Guid Oid { get; set; }

        [Required]
        public Guid UserAccountId { get; set; }

        [Required]
        public int HospitalId { get; set; }

        [ForeignKey("UserAccountId")]
        public UserAccount UserAccount { get; set; }

        [ForeignKey("HospitalId")]
        public Hospital Hospital { get; set; }
    }
}