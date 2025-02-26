using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IPDService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IPDPatientController : ControllerBase
    {
        private readonly IPDDataContext _context;

        public IPDPatientController(IPDDataContext context)
        {
            _context = context;
        }

        // POST: api/IPDPatient
        [HttpPost]
        public async Task<IActionResult> CreateIPDPatient(IPDPatient ipdPatient)
        {
            try
            {
                ipdPatient.IsDeleted = false;
                ipdPatient.IsSynced = false;
                ipdPatient.DateCreated = DateTime.UtcNow;

                _context.IPDPatients.Add(ipdPatient);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetIPDPatientById), new { id = ipdPatient.Oid }, ipdPatient);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/IPDPatient
        [HttpGet]
        public async Task<IActionResult> GetAllIPDPatients()
        {
            try
            {
                var patients = await _context.IPDPatients
                    .Where(p => !p.IsDeleted)
                    .ToListAsync();

                return Ok(patients);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/IPDPatient/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIPDPatientById(Guid id)
        {
            try
            {
                var patient = await _context.IPDPatients
                    .FirstOrDefaultAsync(p => p.Oid == id && !p.IsDeleted);

                if (patient == null)
                {
                    return NotFound($"IPDPatient with ID {id} not found.");
                }

                return Ok(patient);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT: api/IPDPatient/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIPDPatient(Guid id, IPDPatient updatedPatient)
        {
            try
            {
                if (id != updatedPatient.Oid)
                {
                    return BadRequest("IPDPatient ID mismatch.");
                }

                var ipdPatient = await _context.IPDPatients
                    .FirstOrDefaultAsync(p => p.Oid == id && !p.IsDeleted);

                if (ipdPatient == null)
                {
                    return NotFound($"IPDPatient with ID {id} not found.");
                }

                // Update properties
                ipdPatient.PatientId = updatedPatient.PatientId;
                ipdPatient.IsSynced = updatedPatient.IsSynced;
                ipdPatient.DateModified = DateTime.UtcNow;
                // Note: The IPDRecords navigation property is not updated here, as it is typically managed separately

                await _context.SaveChangesAsync();

                return Ok(ipdPatient);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // DELETE: api/IPDPatient/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIPDPatient(Guid id)
        {
            try
            {
                var ipdPatient = await _context.IPDPatients
                    .FirstOrDefaultAsync(p => p.Oid == id && !p.IsDeleted);

                if (ipdPatient == null)
                {
                    return NotFound($"IPDPatient with ID {id} not found.");
                }

                // Soft delete by setting IsDeleted to true
                ipdPatient.IsDeleted = true;
                await _context.SaveChangesAsync();

                return Ok($"IPDPatient with ID {id} has been deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
