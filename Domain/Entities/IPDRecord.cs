using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class IPDRecord : BaseModel
    {
        [Key]
        public Guid Oid { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        [ForeignKey("PatientId")]
        public IPDPatient Patient { get; set; }
    }
}