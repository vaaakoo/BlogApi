using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogServer.Models
{
    public class PostBlog
    {
        [Key]
        public int Id { get; set; }

        // Foreign key to identify the user who created the post
        [ForeignKey("User")]
        public int UserId { get; set; }

        // Navigation property to access the user who created the post
        public User User { get; set; }

        public string? Category { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
