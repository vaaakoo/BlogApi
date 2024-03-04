using Microsoft.AspNetCore.Mvc;
using BlogServer.Models;
using BlogServer.Context;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace BlogServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostBlogController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PostBlogController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/PostBlog
        [HttpPost]
        [Authorize] // Requires authentication to create a post
        public async Task<IActionResult> CreatePost([FromBody] PostBlog post)
        {
            try
            {
                // Retrieve the user ID of the authenticated user
                var userId = int.Parse(User.FindFirst("UserId").Value);

                // Check if the user exists
                var userExists = await _context.Users.FindAsync(userId);
                if (userExists == null)
                {
                    return NotFound("User not found");
                }

                // Set the user ID for the post
                post.UserId = userId;

                // Add the post to the database
                _context.Posts.Add(post);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.Error.WriteLine(ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // GET: api/PostBlog/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(int id)
        {
            // Retrieve the post from the database based on the ID
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound("Post not found");
            }

            return Ok(post);
        }
    }
}
