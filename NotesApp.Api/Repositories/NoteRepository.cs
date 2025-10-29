using Dapper;
using NotesApp.Api.Data;
using NotesApp.Api.Models;

namespace NotesApp.Api.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly DapperContext _context;
        public NoteRepository(DapperContext context) => _context = context;

        public async Task<IEnumerable<Note>> GetAllAsync(int userId)
        {
            var sql = "SELECT * FROM Notes WHERE UserId = @UserId ORDER BY CreatedAt DESC";
            using var conn = _context.CreateConnection();
            return await conn.QueryAsync<Note>(sql, new { UserId = userId });
        }
        public async Task<IEnumerable<Note>> GetAllFilteredAsync(int userId, int categoryId)
        {
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Personal" },
                new Category { Id = 2, Name = "Work" }
            };
            var notes = new List<Note>
            {
                new Note { Id = 1, Title = "Health", CategoryId = 2, CreatedAt = DateTime.UtcNow.AddDays(-4) },
                new Note { Id = 2, Title = "Travel", CategoryId = 1, CreatedAt = DateTime.UtcNow.AddDays(-2) },
                new Note { Id = 3, Title = "Chores", CategoryId = 2, CreatedAt = DateTime.UtcNow.AddDays(-1) }
            };

            var filterNote = notes
            .Where(n => n.CategoryId == categoryId && n.CreatedAt <= DateTime.UtcNow.AddDays(-2))
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new Note { Title = n.Title, CreatedAt = n.CreatedAt })
            .ToList();


            // Grouping By



            // foreach (var note in filterNote)
            //     Console.WriteLine("This is the note's title: " + note.Title + " and this is the creation date: " + note.CreatedAt);
            return filterNote;
            // var sql = "SELECT * FROM Notes WHERE UserId = @UserId AND CategoryId = @CategoryId ORDER BY CreatedAt DESC";
            // using var conn = _context.CreateConnection();
            // return await conn.QueryAsync<Note>(sql, new { UserId = userId, CategoryId = categoryId });
        }
        public async Task<Note?> GetByIdAsync(int id, int userId)
        {
            const string sql = @"SELECT * 
                                 FROM Notes 
                                 WHERE Id = @Id AND UserId = @UserId";

            using var conn = _context.CreateConnection();
            return await conn.QuerySingleOrDefaultAsync<Note>(sql, new { Id = id, UserId = userId });
        }

        public async Task<int> CreateAsync(Note note)
        {
            const string sql = @"INSERT INTO Notes (UserId, Title, Content, CreatedAt, UpdatedAt)
                                 VALUES (@UserId, @Title, @Content, GETUTCDATE(), GETUTCDATE());
                                 SELECT CAST(SCOPE_IDENTITY() as int);";

            using var conn = _context.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(sql, note);
        }

        public async Task<bool> UpdateAsync(Note note)
        {
            const string sql = @"UPDATE Notes
                                 SET Title = @Title, 
                                     Content = @Content, 
                                     UpdatedAt = GETUTCDATE()
                                 WHERE Id = @Id AND UserId = @UserId";

            using var conn = _context.CreateConnection();
            var rows = await conn.ExecuteAsync(sql, note);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            const string sql = @"DELETE FROM Notes 
                                 WHERE Id = @Id AND UserId = @UserId";

            using var conn = _context.CreateConnection();
            var rows = await conn.ExecuteAsync(sql, new { Id = id, UserId = userId });
            return rows > 0;
        }
    }
}

