using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Hospital : BaseModel
    {
        [Key]
        public int Oid { get; set; }

        [Required]
        [StringLength(100)]
        public string HospitalName { get; set; }

        [JsonIgnore]
        public virtual IEnumerable<HospitalAccess>? HospitalAccesses { get; set; }
    }
}
