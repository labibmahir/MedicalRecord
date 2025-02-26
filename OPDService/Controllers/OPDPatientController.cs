using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace OPDService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OPDPatientController : ControllerBase
    {
        private readonly OPDDataContext _context;

        public OPDPatientController(OPDDataContext context)
        {
            _context = context;
        }

        // POST: api/OPDPatient
        [HttpPost]
        public async Task<IActionResult> CreateOPDPatient(OPDPatient opdPatient)
        {
            try
            {
                opdPatient.IsDeleted = false;
                opdPatient.IsSynced = false;
                opdPatient.DateCreated = DateTime.UtcNow;
                // Set additional BaseModel properties here if needed (e.g., CreatedBy, CreatedIn)

                _context.OPDPatients.Add(opdPatient);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetOPDPatientById), new { id = opdPatient.Oid }, opdPatient);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/OPDPatient
        [HttpGet]
        public async Task<IActionResult> GetAllOPDPatients()
        {
            try
            {
                var patients = await _context.OPDPatients
                    .Where(p => !p.IsDeleted)
                    .ToListAsync();
                return Ok(patients);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/OPDPatient/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOPDPatientById(Guid id)
        {
            try
            {
                var patient = await _context.OPDPatients
                    .FirstOrDefaultAsync(p => p.Oid == id && !p.IsDeleted);

                if (patient == null)
                {
                    return NotFound($"OPDPatient with ID {id} not found.");
                }

                return Ok(patient);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT: api/OPDPatient/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOPDPatient(Guid id, OPDPatient updatedPatient)
        {
            try
            {
                if (id != updatedPatient.Oid)
                {
                    return BadRequest("OPDPatient ID mismatch.");
                }

                var patient = await _context.OPDPatients
                    .FirstOrDefaultAsync(p => p.Oid == id && !p.IsDeleted);

                if (patient == null)
                {
                    return NotFound($"OPDPatient with ID {id} not found.");
                }

                // Update properties
                patient.PatientId = updatedPatient.PatientId;
                patient.IsSynced = updatedPatient.IsSynced;
                patient.DateModified = DateTime.UtcNow;
                // Update additional BaseModel properties if needed (e.g., ModifiedBy, ModifiedIn)

                await _context.SaveChangesAsync();
                return Ok(patient);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // DELETE: api/OPDPatient/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOPDPatient(Guid id)
        {
            try
            {
                var patient = await _context.OPDPatients
                    .FirstOrDefaultAsync(p => p.Oid == id && !p.IsDeleted);

                if (patient == null)
                {
                    return NotFound($"OPDPatient with ID {id} not found.");
                }

                // Soft delete by marking the record as deleted
                patient.IsDeleted = true;
                await _context.SaveChangesAsync();

                return Ok($"OPDPatient with ID {id} has been deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}