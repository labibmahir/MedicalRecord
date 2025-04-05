using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MongoEntities
{
    public class ReadPatient : BaseModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Oid { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string Age { get; set; }
    }
}
