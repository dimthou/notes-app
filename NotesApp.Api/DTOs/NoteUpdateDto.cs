namespace NotesApp.Api.DTOs
{
    public class NoteUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
    }
}
