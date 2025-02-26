using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaseService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly DataContext _context;

        public ServiceController(DataContext context)
        {
            _context = context;
        }

        // POST: api/Service
        [HttpPost]
        public async Task<IActionResult> CreateService(Service service)
        {
            try
            {
                service.IsDeleted = false;
                service.IsSynced = false;
                service.DateCreated = DateTime.UtcNow;

                _context.Services.Add(service);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetServiceById), new { id = service.Oid }, service);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/Service
        [HttpGet]
        public async Task<IActionResult> GetAllServices()
        {
            try
            {
                var services = await _context.Services.Where(u => !u.IsDeleted)
                    .ToListAsync();

                return Ok(services);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/Service/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            try
            {
                var service = await _context.Services.FindAsync(id);

                if (service == null)
                {
                    return NotFound($"Service with ID {id} not found.");
                }

                return Ok(service);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT: api/Service/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int id, Service updatedService)
        {
            try
            {
                if (id != updatedService.Oid)
                {
                    return BadRequest("Service ID mismatch.");
                }

                var service = await _context.Services.FindAsync(id);
                if (service == null)
                {
                    return NotFound($"Service with ID {id} not found.");
                }

                // Update properties
                service.ServiceName = updatedService.ServiceName;
                updatedService.IsDeleted = false;
                updatedService.IsSynced = false;
                updatedService.DateModified = DateTime.UtcNow;
                // Update additional properties from BaseModel if required (e.g., DateModified)

                await _context.SaveChangesAsync();
                return Ok(service);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // DELETE: api/Service/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            try
            {
                var service = await _context.Services.FindAsync(id);
                if (service == null)
                {
                    return NotFound($"Service with ID {id} not found.");
                }

                _context.Services.Remove(service);
                await _context.SaveChangesAsync();

                return Ok($"Service with ID {id} has been deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}