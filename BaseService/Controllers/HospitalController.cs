using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaseService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HospitalController : ControllerBase
    {
        private readonly DataContext _context;

        public HospitalController(DataContext context)
        {
            _context = context;
        }

        // POST: api/Hospital
        [HttpPost]
        public async Task<IActionResult> CreateHospital(Hospital hospital)
        {
            try
            {
                hospital.IsDeleted = false;
                hospital.IsSynced = false;
                hospital.DateCreated = DateTime.UtcNow;
                // Optionally set other BaseModel properties, e.g. CreatedBy, CreatedIn, etc.

                _context.Hospitals.Add(hospital);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetHospitalById), new { id = hospital.Oid }, hospital);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/Hospital
        [HttpGet]
        public async Task<IActionResult> GetAllHospitals()
        {
            try
            {
                var hospitals = await _context.Hospitals
                    .Where(h => !h.IsDeleted)
                    .ToListAsync();
                return Ok(hospitals);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/Hospital/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHospitalById(int id)
        {
            try
            {
                var hospital = await _context.Hospitals
                    .FirstOrDefaultAsync(h => h.Oid == id && !h.IsDeleted);

                if (hospital == null)
                {
                    return NotFound($"Hospital with ID {id} not found.");
                }

                return Ok(hospital);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT: api/Hospital/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHospital(int id, Hospital updatedHospital)
        {
            try
            {
                if (id != updatedHospital.Oid)
                {
                    return BadRequest("Hospital ID mismatch.");
                }

                var hospital = await _context.Hospitals
                    .FirstOrDefaultAsync(h => h.Oid == id && !h.IsDeleted);

                if (hospital == null)
                {
                    return NotFound($"Hospital with ID {id} not found.");
                }

                // Update properties
                hospital.HospitalName = updatedHospital.HospitalName;
                hospital.IsSynced = updatedHospital.IsSynced;
                hospital.DateModified = DateTime.UtcNow;
                // Optionally update additional BaseModel properties like ModifiedBy, ModifiedIn, etc.

                await _context.SaveChangesAsync();
                return Ok(hospital);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // DELETE: api/Hospital/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHospital(int id)
        {
            try
            {
                var hospital = await _context.Hospitals
                    .FirstOrDefaultAsync(h => h.Oid == id && !h.IsDeleted);

                if (hospital == null)
                {
                    return NotFound($"Hospital with ID {id} not found.");
                }

                // Perform a soft delete by setting IsDeleted to true
                hospital.IsDeleted = true;
                await _context.SaveChangesAsync();

                return Ok($"Hospital with ID {id} has been deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}