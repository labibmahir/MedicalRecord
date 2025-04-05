using Domain.Entities;
using Domain.MongoEntities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Queries.Patients
{
    public record GetAllPatientQuery : IRequest<List<ReadPatient>>;
}
