using Domain.Entities;
using Infrastructure.Commands.Patients.Create;
using Infrastructure.Commands.Patients.Delete;
using Infrastructure.Commands.Patients.Update;
using Infrastructure.Contexts;
using Infrastructure.Queries.Patients;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaseService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IMediator sender;

        public PatientController(IMediator sender)
        {
            this.sender = sender;
        }

        // POST: api/Patient
        [HttpPost]
        public async Task<IActionResult> CreatePatient(CreatePatientCommand command)
        {
            try
            {
                var patient = await sender.Send(command);

                return Ok(patient);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/Patient
        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            try
            {
                var patients = await sender.Send(new GetAllPatientQuery());

                if (patients == null || patients.Count == 0)
                    return NotFound("No patients found.");

                return Ok(patients);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/Patient/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(Guid id)
        {
            try
            {
                var patientInDb = await sender.Send(new GetPatientByIdQuery(id));

                return Ok(patientInDb);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT: api/Patient/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(Guid id, UpdatePatientCommand command)
        {
            try
            {
                if (id != command.oid)
                {
                    return BadRequest("Patient ID mismatch.");
                }

                var updatedPatient = await sender.Send(command);

                if (updatedPatient == null)
                {
                    return NotFound($"Patient with ID {id} not found.");
                }

                return Ok(updatedPatient); // Return the updated patient
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // DELETE: api/Patient/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(Guid id)
        {
            try
            {
                var deletePatient = await sender.Send(new DeletePatientCommand(id));

                if (deletePatient == null)
                    return NotFound($"Patient with ID {id} not found.");

                return Ok(deletePatient); // Return the updated patient
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}