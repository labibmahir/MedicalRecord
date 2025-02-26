using Domain.Entities;
using Infrastructure.Contexts;
using JWTAuthenticationManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BaseService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAccountController : ControllerBase
    {
        private DataContext dataContext;
        private readonly JwtTokenHandler jwtTokenHandler;

        public UserAccountController(DataContext dataContext,JwtTokenHandler jwtTokenHandler)
        {
            this.jwtTokenHandler = jwtTokenHandler;
            this.dataContext = dataContext;
        }

        [HttpPost]
        [Route("user")]
        public async Task<IActionResult> CreateUserAccount(UserAccount user)
        {
            try
            {
                user.IsDeleted = false;
                user.IsSynced = false;
                user.DateCreated = DateTime.UtcNow;

                dataContext.Add(user);
                await dataContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status201Created, user);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await dataContext.UserAccounts
                    .Where(u => !u.IsDeleted)
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("user/{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var user = await dataContext.UserAccounts
                    .FirstOrDefaultAsync(u => u.Oid == id && !u.IsDeleted);

                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Route("user/{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, UserAccount updatedUser)
        {
            try
            {
                var existingUser = await dataContext.UserAccounts
                    .FirstOrDefaultAsync(u => u.Oid == id && !u.IsDeleted);

                if (existingUser == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                // Update properties
                existingUser.FirstName = updatedUser.FirstName;
                existingUser.Surname = updatedUser.Surname;
                existingUser.Email = updatedUser.Email;
                existingUser.IsDeleted = false;
                existingUser.IsSynced = false;
                existingUser.DateModified = DateTime.UtcNow;

                await dataContext.SaveChangesAsync();

                return Ok(existingUser);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        [Route("user/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var user = await dataContext.UserAccounts
                    .FirstOrDefaultAsync(u => u.Oid == id && !u.IsDeleted);

                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                // Soft delete
                user.IsDeleted = true;
                await dataContext.SaveChangesAsync();

                return Ok($"User with ID {id} has been deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<AuthenticationResponse?>> Login([FromBody] AuthenticationRequest authenticationRequest)
        {
            try
            {
                var user = await jwtTokenHandler.GenerateJwtToken(authenticationRequest);

                if (user == null)
                    return Unauthorized();

                return user;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}