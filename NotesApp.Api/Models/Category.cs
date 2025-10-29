namespace NotesApp.Api.Models{
    public class Category{
        public int Id { get; set; }
        public required string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}