namespace NotesApp.Api.DTOs
{
    public class NoteCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
    }
}
