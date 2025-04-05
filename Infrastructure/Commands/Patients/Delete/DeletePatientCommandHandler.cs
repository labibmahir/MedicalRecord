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

namespace Infrastructure.Commands.Patients.Delete
{
    public class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand, Patient>
    {
        private readonly DataContext dataContext;

        public DeletePatientCommandHandler(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<Patient> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var patient = await dataContext.Patients
                                   .FirstOrDefaultAsync(p => p.Oid == request.id && p.IsDeleted == false, cancellationToken);

                if (patient == null)
                    throw new Exception("Patient not found.");

                dataContext.Patients.Remove(patient);
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
