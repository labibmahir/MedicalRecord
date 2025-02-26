using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaseService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IPDDataContext _ipd;
        private readonly OPDDataContext _opd;

        public PatientController(DataContext context, IPDDataContext ipd, OPDDataContext opd)
        {
            _context = context;
            _ipd = ipd;
            _opd = opd;
        }

        // POST: api/Patient
        [HttpPost]
        public async Task<IActionResult> CreatePatient(Patient patient)
        {
            try
            {
                Guid oid = new Guid();

                patient.Oid = oid;
                patient.IsDeleted = false;
                patient.IsSynced = false;
                patient.DateCreated = DateTime.UtcNow;
                // Set additional BaseModel properties if needed (e.g., CreatedBy, CreatedIn)

                _context.Patients.Add(patient);

                IPDPatient ipdPatient = new IPDPatient();
                ipdPatient.Oid = new Guid();
                ipdPatient.PatientId = oid;
                ipdPatient.IsDeleted = false;
                ipdPatient.IsSynced = false;
                ipdPatient.DateCreated = DateTime.UtcNow;

                _ipd.IPDPatients.Add(ipdPatient);


                OPDPatient oPDPatient = new OPDPatient();
                oPDPatient.Oid = new Guid();
                oPDPatient.PatientId = oid;
                oPDPatient.IsDeleted = false;
                oPDPatient.IsSynced = false;
                oPDPatient.DateCreated = DateTime.UtcNow;

                _opd.OPDPatients.Add(oPDPatient);

                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPatientById), new { id = patient.Oid }, patient);
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
                var patients = await _context.Patients
                    .Where(p => !p.IsDeleted)
                    .ToListAsync();
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
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.Oid == id && !p.IsDeleted);
                if (patient == null)
                {
                    return NotFound($"Patient with ID {id} not found.");
                }
                return Ok(patient);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT: api/Patient/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(Guid id, Patient updatedPatient)
        {
            try
            {
                if (id != updatedPatient.Oid)
                {
                    return BadRequest("Patient ID mismatch.");
                }

                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.Oid == id && !p.IsDeleted);

                if (patient == null)
                {
                    return NotFound($"Patient with ID {id} not found.");
                }

                // Update properties
                patient.FirstName = updatedPatient.FirstName;
                patient.Surname = updatedPatient.Surname;
                patient.Age = updatedPatient.Age;
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

        // DELETE: api/Patient/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(Guid id)
        {
            try
            {
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.Oid == id && !p.IsDeleted);

                if (patient == null)
                {
                    return NotFound($"Patient with ID {id} not found.");
                }

                // Soft delete by setting IsDeleted to true
                patient.IsDeleted = true;
                await _context.SaveChangesAsync();

                return Ok($"Patient with ID {id} has been deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}