using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Commands.Patients.Update
{
    public record UpdatePatientCommand(Guid oid, string firstname, string surname, string age) : IRequest<Patient>;
}
