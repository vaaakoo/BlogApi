using Microsoft.AspNetCore.Mvc;
using BlogServer.Models;
using BlogServer.Context;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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

        [HttpPost]
        [Authorize] // Requires authentication
        public IActionResult CreatePost([FromBody] PostBlog post)
        {
            try
            {
                // Get the ID of the authenticated user
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

                // Set the UserId property of the post to the authenticated user's ID
                post.UserId = userId;

                // Add the post to the database
                _context.Posts.Add(post);
                _context.SaveChanges();

                return Ok(post);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Failed to create post", Error = ex.Message });
            }
        }

        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetPostsByCategory(string category)
        {
            // Retrieve posts from the database based on the category
            var posts = await _context.Posts.Where(p => p.Category == category).ToListAsync();
            if (posts == null || !posts.Any())
            {
                return NotFound("No posts found for the specified category");
            }

            return Ok(posts);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPostsByUserId(string userId)
        {
            // Retrieve posts from the database based on the userId
            var posts = await _context.Posts.Where(p => p.UserId == userId).ToListAsync();
            if (posts == null || !posts.Any())
            {
                return NotFound("No posts found for the specified user");
            }

            return Ok(posts);
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
