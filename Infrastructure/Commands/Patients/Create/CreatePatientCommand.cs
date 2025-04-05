using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Commands.Patients.Create
{
    public record CreatePatientCommand(string firstname, string surname, string age) : IRequest<Patient>;
}
