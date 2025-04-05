using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using MediatR;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Commands.Patients.Create
{
    public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, Patient>
    {
        private readonly DataContext dataContext;

        public CreatePatientCommandHandler(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }


        public async Task<Patient> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var patient = new Patient
                {
                    Oid = Guid.NewGuid(),
                    FirstName = request.firstname,
                    Surname = request.surname,
                    Age = request.age,
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = false,
                    IsSynced = false
                };

                dataContext.Patients.Add(patient);

                await dataContext.SaveChangesAsync();

                return patient;
            }
            catch(Exception ex)
            {
                throw ex.InnerException;
            }
        }
    }
}
