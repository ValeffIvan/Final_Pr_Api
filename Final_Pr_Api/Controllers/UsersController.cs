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
                var userExist = _context.Users.FirstOrDefault(u => u.email == user.email);

                if (userExist != null)
                {
                    return NotFound(new { status = 400, message = "El usuario ya existe" });
                }
                user.createTime = DateTime.Now;
                user.password = AuthService.HashPassword(user.password);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return Ok(new { status = 200, message = "Usuario creado con exito"});               

            }catch(Exception ex)
            {
                return BadRequest(new { status = 500, message = ex.Message });
            }
        }
       
        [HttpGet, Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _context.Users
                    .Join(_context.Roles,
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
                return BadRequest(new { status = 500, message = ex.Message });
            }
        }



        [HttpPut("ChangePassword/{id}"), Authorize]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] string password)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { status = 404, message = "Usuario no encontrado" });
                }

                var newPassword = AuthService.HashPassword(password);
                user.password = newPassword;
                await _context.SaveChangesAsync();

                return Ok(new { status = 200, message = "Contraseña cambiada exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 500, message = ex.Message });
            }
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> EditUser(int id, [FromBody] User userData)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { status = 404, message = "Usuario no encontrado" });
                }

                if (!string.IsNullOrEmpty(userData.password))
                {
                    user.password = AuthService.HashPassword(userData.password);
                }

                if (!string.IsNullOrEmpty(userData.username))
                {
                    user.username = userData.username;
                }

                if (!string.IsNullOrEmpty(userData.email))
                {
                    user.email = userData.email;
                }

                if (userData.idRol != 0)
                {
                    user.idRol = userData.idRol;
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return Ok(new { status = 200, message = "Usuario actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 500, message = ex.Message });
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
                    return NotFound(new { status = 404, message = "Usuario no encontrado" });
                }

                var commentsToDelete = _context.Comments.Where(c => c.authorId == id);

                var postsToDelete = _context.Posts.Where(p => p.authorId == id);

                _context.Comments.RemoveRange(commentsToDelete);

                _context.Posts.RemoveRange(postsToDelete);

                _context.Users.Remove(user);

                await _context.SaveChangesAsync();

                return Ok(new { status = 200, message = "Usuario y sus posts y comentarios eliminados con éxito" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 500, message = ex.Message });
            }
        }

        [HttpGet("byEmail/{email}"), Authorize]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                var user = await _context.Users
                    .Where(u => u.email == email)
                    .Join(_context.Roles,
                          user => user.idRol,
                          role => role.idRol,
                          (user, role) => new { User = user, RolName = role.name })
                    .FirstOrDefaultAsync();

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
                    return NotFound(new { status = 404, message = "Usuario no encontrado" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 500, message = ex.Message });
            }
        }


    }
}
