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
                post.createTime = DateTime.Now;
                _context.Posts.Add(post);
                await _context.SaveChangesAsync();
                return Ok();
            }catch(Exception ex)
            {
                return BadRequest(new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPost()
        {
            try
            {
                var posts = await _context.Posts
                    .Join(_context.Users,
                          post => post.authorId,
                          user => user.idUsers,
                          (post, user) => new
                          {
                              idPost = post.idPost,
                              title = post.title,
                              description = post.description,
                              authorId = post.authorId,
                              AuthorUsername = user.username,
                              createTime = post.createTime,
                          })
                    .OrderByDescending(post => post.createTime)
                    .ToListAsync();

                return Ok(posts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 500, message = ex.Message });
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
                    return NotFound(new { status = 404, message = "Post no encontrado" });
                }
                else
                {
                    post.title = newPost.title;
                    post.description = newPost.description;
                    await _context.SaveChangesAsync();

                    return Ok(new {status = 200, message = "Post modificado con exito"});
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 500, message = ex.Message });
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
                    return NotFound(new {status= 404, message= "Post no encontrado"});
                }

                var comments = _context.Comments.Where(c => c.postId == id);
                _context.Comments.RemoveRange(comments);

                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();

                return Ok(new {status=200, message="Post eliminado con exito"});
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 500, message = ex.Message });
            }
        }

        [HttpGet("byauthor/{authorId}")]
        public async Task<IActionResult> GetPostsByAuthor(int authorId)
        {
            try
            {
                var postsWithAuthorUsername = await _context.Posts
                    .Where(p => p.authorId == authorId)
                    .Join(_context.Users,
                          post => post.authorId,
                          user => user.idUsers,
                          (post, user) => new
                          {
                              idPost = post.idPost,
                              title = post.title,
                              description = post.description,
                              authorId = post.authorId,
                              AuthorUsername = user.username,
                              createTime = post.createTime,
                          })
                    .OrderByDescending(post => post.createTime)
                    .ToListAsync();

                return Ok(postsWithAuthorUsername);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 500, message = ex.Message });
            }
        }


    }
}
