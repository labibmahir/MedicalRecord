using Domain.Entities;
using Domain.MongoEntities;
using Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Queries.Patients
{
    public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, ReadPatient?>
    {
        private readonly IMongoCollection<ReadPatient> mongoCollection;

        public GetPatientByIdQueryHandler(IMongoCollection<ReadPatient> mongoCollection)
        {
            this.mongoCollection = mongoCollection;
        }

        public async Task<ReadPatient?> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var filter = Builders<ReadPatient>.Filter.Eq(p => p.Oid, request.id) & Builders<ReadPatient>.Filter.Eq(p => p.IsDeleted, false);

                var patientInDb = await mongoCollection.Find(filter).FirstOrDefaultAsync();

                return patientInDb;
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }
    }
}