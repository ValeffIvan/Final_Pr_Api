using Final_Pr_Api.Data;
using Final_Pr_Api.Models;
using Humanizer.Localisation.DateToOrdinalWords;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Final_Pr_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PostsController(AppDbContext appDbContext) {
            _context = appDbContext;
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> AddPost(Post post)
        {
            try
            {
                _context.Posts.Add(post);
                await _context.SaveChangesAsync();
                return Ok();
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPost()
        {
            try
            {
                var posts = await _context.Posts.ToListAsync();
                return Ok(posts);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] Post newPost)
        {
            try
            {
                var post = await _context.Posts.FindAsync(id);

                if (post == null)
                {
                    return BadRequest();
                }
                else
                {
                    post.title = newPost.title;
                    post.description = newPost.description;
                    await _context.SaveChangesAsync();

                    return Ok(post);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeletePost(int id)
        {
            try
            {
                var post = await _context.Posts.FindAsync(id);

                if (post == null)
                {
                    return NotFound();
                }

                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("byauthor/{authorId}"), Authorize]
        public IActionResult GetPostsByAuthor(int authorId)
        {
            try
            {
                var posts = _context.Posts
                    .Where(p => p.authorId == authorId)
                    .ToList();

                return Ok(posts); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }


    }
}
