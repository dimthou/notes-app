using System;
using System.ComponentModel.DataAnnotations;

namespace NotesApp.Api.Models
{
    public class Note
    {
        public int Id { get; set; }

        public int UserId { get; set; }   // FK to Users

        [Required(ErrorMessage = "Title is required")]
        [MinLength(1, ErrorMessage = "Title cannot be empty")]
        public required string Title { get; set; }

        public string? Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
