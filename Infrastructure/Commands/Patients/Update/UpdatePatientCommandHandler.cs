using Domain.Entities;
using Infrastructure.Commands.Patients.Create;
using Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Commands.Patients.Update
{
    public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, Patient>
    {
        private readonly DataContext dataContext;

        public UpdatePatientCommandHandler(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<Patient> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var patient = await dataContext.Patients
                    .FirstOrDefaultAsync(p => p.Oid == request.oid && p.IsDeleted == false, cancellationToken);

                if (patient == null)
                    throw new Exception("Patient not found.");

                // Update patient details
                patient.FirstName = request.firstname;
                patient.Surname = request.surname;
                patient.Age = request.age;
                patient.IsDeleted = false;
                patient.IsSynced = false;
                patient.DateModified = DateTime.UtcNow;

                dataContext.Patients.Update(patient);
                await dataContext.SaveChangesAsync();

                return patient;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the patient.", ex);
            }
        }
    }
}