﻿using System.ComponentModel.DataAnnotations;

namespace BlogServer.Models
{
    public class PostBlog
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string? Category { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
