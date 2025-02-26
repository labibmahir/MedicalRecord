using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class BaseModel
    {
        public int? CreatedIn {  get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? DateCreated { get; set; }

        public int? ModifiedIn { get; set; }

        public Guid? ModifiedBy { get; set; }

        public DateTime? DateModified { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsSynced { get; set; }
    }
}