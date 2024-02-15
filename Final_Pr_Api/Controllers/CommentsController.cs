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
                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
                return Ok();
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllComments()
        {
            try
            {
                var posts = await _context.Comments.ToListAsync();
                return Ok(posts);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}"), Authorize]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] Comment newComment)
        {
            try
            {
                var comment = await _context.Comments.FindAsync(id);

                if (comment == null)
                {
                    return BadRequest();
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
                return BadRequest(ex.Message);
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
                    return NotFound();
                }

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("bypost/{postId}")]
        public IActionResult GetCommentsByPost(int postId)
        {
            try
            {
                var comments = _context.Comments
                    .Where(p => p.postId == postId)
                    .ToList();

                return Ok(comments); 
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
