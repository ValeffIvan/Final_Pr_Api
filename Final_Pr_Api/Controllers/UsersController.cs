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
                var users = await _context.Users.ToListAsync();
                return Ok(users);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
        //Transformarlo en un cambiar contraseña que traiga la contra vieja y la nueva para poder cambiarla
       /*[HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsers(int id, [FromBody] User newUser)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return BadRequest();
                }
                else
                {
                    user.title = newPost.title;
                    user.description = newPost.description;
                    await _context.SaveChangesAsync();

                    return Ok(post);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }*/

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

        [HttpGet("byEmail/{Email}"), Authorize]
        public IActionResult GetUserById(string Email)
        {
            try
            {
                var user = _context.Users
                    .FirstOrDefault(u => u.email == Email);

                return Ok(user); 
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
