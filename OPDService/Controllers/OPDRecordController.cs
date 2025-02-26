using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace OPDService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OPDRecordController : ControllerBase
    {
        private readonly OPDDataContext _context;

        public OPDRecordController(OPDDataContext context)
        {
            _context = context;
        }

        // POST: api/OPDRecord
        [HttpPost]
        public async Task<IActionResult> CreateOPDRecord(OPDRecord record)
        {
            try
            {
                record.IsDeleted = false;
                record.IsSynced = false;
                record.DateCreated = DateTime.UtcNow;

                _context.OPDRecords.Add(record);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetOPDRecordById), new { id = record.Oid }, record);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/OPDRecord
        [HttpGet]
        public async Task<IActionResult> GetAllOPDRecords()
        {
            try
            {
                var records = await _context.OPDRecords
                    .Where(r => !r.IsDeleted)
                    .Include(r => r.Patient)
                    .ToListAsync();
                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/OPDRecord/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOPDRecordById(Guid id)
        {
            try
            {
                var record = await _context.OPDRecords
                    .Include(r => r.Patient)
                    .FirstOrDefaultAsync(r => r.Oid == id && !r.IsDeleted);

                if (record == null)
                {
                    return NotFound($"OPDRecord with ID {id} not found.");
                }
                return Ok(record);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT: api/OPDRecord/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOPDRecord(Guid id, OPDRecord updatedRecord)
        {
            try
            {
                if (id != updatedRecord.Oid)
                {
                    return BadRequest("OPDRecord ID mismatch.");
                }

                var record = await _context.OPDRecords
                    .FirstOrDefaultAsync(r => r.Oid == id && !r.IsDeleted);

                if (record == null)
                {
                    return NotFound($"OPDRecord with ID {id} not found.");
                }

                // Update the properties
                record.ServiceId = updatedRecord.ServiceId;
                record.PatientId = updatedRecord.PatientId;
                record.IsSynced = updatedRecord.IsSynced;
                record.DateModified = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return Ok(record);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // DELETE: api/OPDRecord/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOPDRecord(Guid id)
        {
            try
            {
                var record = await _context.OPDRecords
                    .FirstOrDefaultAsync(r => r.Oid == id && !r.IsDeleted);

                if (record == null)
                {
                    return NotFound($"OPDRecord with ID {id} not found.");
                }

                // Soft delete: mark the record as deleted
                record.IsDeleted = true;
                await _context.SaveChangesAsync();

                return Ok($"OPDRecord with ID {id} has been deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}