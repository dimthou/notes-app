// using NotesApp.Api.Models;

// namespace NotesApp.Api.Repositories
// {
//     public interface INoteRepository
//     {
//         Task<IEnumerable<Note>> GetAllAsync(int userId);
//         Task<Note?> GetByIdAsync(int id, int userId);
//         Task<int> CreateAsync(Note note);
//         Task<bool> UpdateAsync(Note note);
//         Task<bool> DeleteAsync(int id, int userId);
//     }
// }
using NotesApp.Api.Models;

namespace NotesApp.Api.Repositories
{
    public interface INoteRepository
    {
        Task<IEnumerable<Note>> GetAllAsync(int userId);
        Task<Note?> GetByIdAsync(int id, int userId);
        Task<int> CreateAsync(Note note);
        Task<bool> UpdateAsync(Note note);
        Task<bool> DeleteAsync(int id, int userId);
    }
}
