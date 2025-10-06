using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesApp.Api.DTOs;
using NotesApp.Api.Models;
using NotesApp.Api.Repositories;

namespace NotesApp.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly INoteRepository _repo;
        public NotesController(INoteRepository repo) => _repo = repo;

        private int GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID claim missing");
            return int.Parse(userId);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();
            var notes = await _repo.GetAllAsync(userId);
            var result = notes.Select(n => new
            {
                Id = n.Id,
                Title = n.Title,
                CreatedAt = n.CreatedAt
            });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetUserId();
            var note = await _repo.GetByIdAsync(id, userId);
            if (note == null) return NotFound();
            var readDto = new NoteReadDto
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt
            };
            return Ok(readDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(NoteCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                return BadRequest(new { error = "Title is required." });
            }
            var userId = GetUserId();
            var note = new Note
            {
                UserId = userId,
                Title = dto.Title.Trim(),
                Content = dto.Content
            };
            var id = await _repo.CreateAsync(note);
            note.Id = id;
            return CreatedAtAction(nameof(GetById), new { id = note.Id }, note);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, NoteUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                return BadRequest(new { error = "Title is required." });
            }

            var userId = GetUserId();
            var note = await _repo.GetByIdAsync(id, userId);
            if (note == null) return NotFound();

            note.Title = dto.Title.Trim();
            note.Content = dto.Content;

            var updated = await _repo.UpdateAsync(note);
            return updated ? NoContent() : StatusCode(500);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var deleted = await _repo.DeleteAsync(id, userId);
            return deleted ? NoContent() : NotFound();
        }
    }
}

