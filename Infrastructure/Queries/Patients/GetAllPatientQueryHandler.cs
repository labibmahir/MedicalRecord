using Domain.Entities;
using Domain.MongoEntities;
using Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Queries.Patients
{
    public class GetAllPatientQueryHandler : IRequestHandler<GetAllPatientQuery, List<ReadPatient>>
    {
        private readonly IMongoCollection<ReadPatient> _mongoCollection;

        public GetAllPatientQueryHandler(IMongoCollection<ReadPatient> mongoCollection)
        {
            _mongoCollection = mongoCollection;
        }

        public async Task<List<ReadPatient>> Handle(GetAllPatientQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Use MongoDB's Find method to retrieve patients where IsDeleted is false
                var filter = Builders<ReadPatient>.Filter.Eq(p => p.IsDeleted, false);
                var patients = await _mongoCollection
                    .Find(filter)
                    .ToListAsync(cancellationToken);  // Get the list of patients from MongoDB

                return patients;
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;  // Ensure exception is thrown with the proper message
            }
        }
    }
}
