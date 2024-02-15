using Final_Pr_Api.Data;
using Final_Pr_Api.Models;
using Final_Pr_Api.Services;
using Humanizer.Localisation.DateToOrdinalWords;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Final_Pr_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UsersController(AppDbContext appDbContext) {
            _context = appDbContext;
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(User user)
        {
            try
            {
                user.password = AuthService.HashPassword(user.password);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return Ok();
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
       
        [HttpGet, Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _context.Users
                    .Join(_context.Rol,
                        user => user.idRol,
                        role => role.idRol,
                        (user, role) => new { User = user, RolName = role.name })
                    .Select(u => new UserDetails {
                        idUsers = u.User.idUsers,
                        username = u.User.username,
                        email = u.User.email,
                        createTime = u.User.createTime,
                        Role = u.RolName
                    })
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] string password)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound($"Usuario no encontrado");
                }

                var newPassword = AuthService.HashPassword(password);
                user.password = newPassword;
                await _context.SaveChangesAsync();

                return Ok("Contraseña cambiada exitosamente");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al cambiar la contraseña: {ex.Message}");
            }
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound();
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("byEmail/{email}"), Authorize]
        public IActionResult GetUserByEmail(string email)
        {
            try
            {
                var user = _context.Users
                    .Where(u => u.email == email)
                    .Join(_context.Rol,
                          user => user.idRol,
                          role => role.idRol,
                          (user, role) => new { User = user, RolName = role.name })
                    .FirstOrDefault();

                if (user != null)
                {
                    var userWithRol = new UserDetails 
                    { 
                        idUsers = user.User.idUsers,
                        username = user.User.username,
                        email = user.User.email,
                        createTime = user.User.createTime,
                        Role = user.RolName,                        
                    };

                    return Ok(userWithRol);
                }
                else
                {
                    return NotFound("Usuario no encontrado.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
