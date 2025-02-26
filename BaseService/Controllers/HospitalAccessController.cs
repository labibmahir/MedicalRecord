using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaseService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HospitalAccessController : ControllerBase
    {
        private readonly DataContext _context;

        public HospitalAccessController(DataContext context)
        {
            _context = context;
        }

        // POST: api/HospitalAccess
        [HttpPost]
        public async Task<IActionResult> CreateHospitalAccess(HospitalAccess access)
        {
            try
            {
                access.IsDeleted = false;
                access.IsSynced = false;
                access.DateCreated = DateTime.UtcNow;
                // Optionally set additional BaseModel properties like CreatedBy or CreatedIn

                _context.HospitalAccess.Add(access);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetHospitalAccessById), new { id = access.Oid }, access);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/HospitalAccess
        [HttpGet]
        public async Task<IActionResult> GetAllHospitalAccesses()
        {
            try
            {
                var accesses = await _context.HospitalAccess
                    .Where(a => !a.IsDeleted)
                    .Include(a => a.UserAccount)
                    .Include(a => a.Hospital)
                    .ToListAsync();

                return Ok(accesses);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/HospitalAccess/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHospitalAccessById(Guid id)
        {
            try
            {
                var access = await _context.HospitalAccess
                    .Include(a => a.UserAccount)
                    .Include(a => a.Hospital)
                    .FirstOrDefaultAsync(a => a.Oid == id && !a.IsDeleted);

                if (access == null)
                {
                    return NotFound($"HospitalAccess with ID {id} not found.");
                }

                return Ok(access);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT: api/HospitalAccess/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHospitalAccess(Guid id, HospitalAccess updatedAccess)
        {
            try
            {
                if (id != updatedAccess.Oid)
                {
                    return BadRequest("HospitalAccess ID mismatch.");
                }

                var access = await _context.HospitalAccess
                    .FirstOrDefaultAsync(a => a.Oid == id && !a.IsDeleted);

                if (access == null)
                {
                    return NotFound($"HospitalAccess with ID {id} not found.");
                }

                // Update properties
                access.UserAccountId = updatedAccess.UserAccountId;
                access.HospitalId = updatedAccess.HospitalId;
                access.IsDeleted = false;
                access.IsSynced = false;
                access.DateModified = DateTime.UtcNow;
                // Optionally update additional BaseModel properties like ModifiedBy or ModifiedIn

                await _context.SaveChangesAsync();
                return Ok(access);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // DELETE: api/HospitalAccess/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHospitalAccess(Guid id)
        {
            try
            {
                var access = await _context.HospitalAccess
                    .FirstOrDefaultAsync(a => a.Oid == id && !a.IsDeleted);

                if (access == null)
                {
                    return NotFound($"HospitalAccess with ID {id} not found.");
                }

                // Soft delete by setting IsDeleted to true
                access.IsDeleted = true;
                await _context.SaveChangesAsync();

                return Ok($"HospitalAccess with ID {id} has been deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}