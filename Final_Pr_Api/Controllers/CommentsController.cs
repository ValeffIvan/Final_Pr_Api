using Final_Pr_Api.Data;
using Final_Pr_Api.Models;
using Humanizer.Localisation.DateToOrdinalWords;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace Final_Pr_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CommentsController(AppDbContext appDbContext) {
            _context = appDbContext;
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> AddComment(Comment comment)
        {
            try
            {
                comment.createTime = DateTime.Now;
                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
                return Ok(new {status= 200, message = "Comentario creado con exito"});
            }catch(Exception ex)
            {
                return BadRequest(new { status = 500, message = ex.Message });
            }
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] Comment newComment)
        {
            try
            {
                var comment = await _context.Comments.FindAsync(id);

                if (comment == null)
                {
                    return NotFound(new { status = 404, message = "Comentario no encontrado" });
                }
                else
                {
                    comment.text = newComment.text;
                    await _context.SaveChangesAsync();

                    return Ok(comment);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 500, message = ex.Message });
            }
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                var comment = await _context.Comments.FindAsync(id);

                if (comment == null)
                {
                    return NotFound(new { status = 404, message = "Comentario no encontrado" });
                }

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();

                return Ok(new {status = 200, message = "Comentario eliminado con exito"});
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 500, message = ex.Message });
            }
        }

        [HttpGet("bypost/{postId}")]
        public IActionResult GetCommentsByPost(int postId)
        {
            try
            {
                var commentsWithUsernames = _context.Comments
                    .Where(c => c.postId == postId)
                    .Join(_context.Users,
                          comment => comment.authorId,
                          user => user.idUsers,
                          (comment, user) => new
                          {
                              Comment = comment,
                              AuthorUsername = user.username
                          })
                    .Select(c => new
                    {
                        c.Comment.authorId,
                        c.Comment.idComment,
                        c.Comment.text,
                        c.Comment.createTime,
                        c.AuthorUsername
                    })
                    .ToList();

                return Ok(commentsWithUsernames);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 500, message = ex.Message });
            }
        }



    }
}
