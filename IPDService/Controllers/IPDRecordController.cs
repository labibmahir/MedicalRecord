using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IPDService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IPDRecordController : ControllerBase
    {
        private readonly IPDDataContext _context;

        public IPDRecordController(IPDDataContext context)
        {
            _context = context;
        }

        // POST: api/IPDRecord
        [HttpPost]
        public async Task<IActionResult> CreateIPDRecord(IPDRecord record)
        {
            try
            {
                record.IsDeleted = false;
                record.IsSynced = false;
                record.DateCreated = DateTime.UtcNow;

                _context.IPDRecords.Add(record);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetIPDRecordById), new { id = record.Oid }, record);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/IPDRecord
        [HttpGet]
        public async Task<IActionResult> GetAllIPDRecords()
        {
            try
            {
                var records = await _context.IPDRecords
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

        // GET: api/IPDRecord/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIPDRecordById(Guid id)
        {
            try
            {
                var record = await _context.IPDRecords
                    .Include(r => r.Patient)
                    .FirstOrDefaultAsync(r => r.Oid == id && !r.IsDeleted);

                if (record == null)
                {
                    return NotFound($"IPDRecord with ID {id} not found.");
                }

                return Ok(record);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT: api/IPDRecord/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIPDRecord(Guid id, IPDRecord updatedRecord)
        {
            try
            {
                if (id != updatedRecord.Oid)
                {
                    return BadRequest("IPDRecord ID mismatch.");
                }

                var record = await _context.IPDRecords
                    .FirstOrDefaultAsync(r => r.Oid == id && !r.IsDeleted);

                if (record == null)
                {
                    return NotFound($"IPDRecord with ID {id} not found.");
                }

                // Update properties
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

        // DELETE: api/IPDRecord/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIPDRecord(Guid id)
        {
            try
            {
                var record = await _context.IPDRecords
                    .FirstOrDefaultAsync(r => r.Oid == id && !r.IsDeleted);

                if (record == null)
                {
                    return NotFound($"IPDRecord with ID {id} not found.");
                }

                // Soft delete by marking the record as deleted
                record.IsDeleted = true;
                await _context.SaveChangesAsync();

                return Ok($"IPDRecord with ID {id} has been deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
