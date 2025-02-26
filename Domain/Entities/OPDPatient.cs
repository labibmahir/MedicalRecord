using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OPDPatient : BaseModel
    {
        [Key]
        public Guid Oid { get; set; }

        public Guid PatientId { get; set; }

        [JsonIgnore]
        public virtual IEnumerable<OPDRecord>? OPDRecords { get; set; }
    }
}
