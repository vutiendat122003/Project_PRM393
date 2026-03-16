using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentLifeApp_BE.Models;

namespace StudentLifeApp_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly LmsDbContext _context;

        public NotesController(LmsDbContext context)
        {
            _context = context;
        }

        // GET: api/notes
        // Get all notes for current user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetNotes([FromQuery] int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("User ID is required");
            }

            var notes = await _context.Notes
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.IsPinned)
                .ThenByDescending(n => n.UpdatedAt)
                .Select(n => new
                {
                    noteId = n.NoteId,
                    userId = n.UserId,
                    title = n.Title,
                    content = n.Content,
                    isPinned = n.IsPinned,
                    createdAt = n.CreatedAt,
                    updatedAt = n.UpdatedAt
                })
                .ToListAsync();

            return Ok(notes);
        }

        // GET: api/notes/5
        // Get single note by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetNote(int id, [FromQuery] int userId)
        {
            var note = await _context.Notes
                .Where(n => n.NoteId == id && n.UserId == userId)
                .Select(n => new
                {
                    noteId = n.NoteId,
                    userId = n.UserId,
                    title = n.Title,
                    content = n.Content,
                    isPinned = n.IsPinned,
                    createdAt = n.CreatedAt,
                    updatedAt = n.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (note == null)
            {
                return NotFound(new { message = "Note not found or you don't have permission to view it" });
            }

            return Ok(note);
        }

        // POST: api/notes
        // Create new note
        [HttpPost]
        public async Task<ActionResult<object>> CreateNote([FromBody] CreateNoteDTO noteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var note = new Note
            {
                UserId = noteDto.UserId,
                Title = noteDto.Title ?? string.Empty,
                Content = noteDto.Content ?? string.Empty,
                IsPinned = noteDto.IsPinned ?? false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            var createdNote = new
            {
                noteId = note.NoteId,
                userId = note.UserId,
                title = note.Title,
                content = note.Content,
                isPinned = note.IsPinned,
                createdAt = note.CreatedAt,
                updatedAt = note.UpdatedAt
            };

            return CreatedAtAction(nameof(GetNote), new { id = note.NoteId, userId = note.UserId }, createdNote);
        }

        // PUT: api/notes/5
        // Update existing note
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNote(int id, [FromBody] UpdateNoteDTO noteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.NoteId == id && n.UserId == noteDto.UserId);

            if (note == null)
            {
                return NotFound(new { message = "Note not found or you don't have permission to update it" });
            }

            // Update fields
            if (noteDto.Title != null) note.Title = noteDto.Title;
            if (noteDto.Content != null) note.Content = noteDto.Content;
            if (noteDto.IsPinned.HasValue) note.IsPinned = noteDto.IsPinned.Value;
            note.UpdatedAt = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { message = "Error updating note" });
            }

            return Ok(new
            {
                noteId = note.NoteId,
                userId = note.UserId,
                title = note.Title,
                content = note.Content,
                isPinned = note.IsPinned,
                createdAt = note.CreatedAt,
                updatedAt = note.UpdatedAt
            });
        }

        // DELETE: api/notes/5
        // Delete note
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id, [FromQuery] int userId)
        {
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.NoteId == id && n.UserId == userId);

            if (note == null)
            {
                return NotFound(new { message = "Note not found or you don't have permission to delete it" });
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Note deleted successfully", noteId = id });
        }

        // PATCH: api/notes/5/pin
        // Toggle pin status
        [HttpPatch("{id}/pin")]
        public async Task<IActionResult> TogglePin(int id, [FromQuery] int userId)
        {
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.NoteId == id && n.UserId == userId);

            if (note == null)
            {
                return NotFound(new { message = "Note not found or you don't have permission to update it" });
            }

            note.IsPinned = note.IsPinned.HasValue ? !note.IsPinned.Value : true;
            note.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                noteId = note.NoteId,
                isPinned = note.IsPinned,
                message = (note.IsPinned ?? false) ? "Note pinned" : "Note unpinned"
            });
        }

        // GET: api/notes/search
        // Search notes by title or content
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<object>>> SearchNotes(
            [FromQuery] int userId,
            [FromQuery] string query)
        {
            if (userId <= 0)
            {
                return BadRequest("User ID is required");
            }

            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query is required");
            }

            var notes = await _context.Notes
                .Where(n => n.UserId == userId &&
                    (n.Title != null && n.Title.Contains(query) ||
                     n.Content != null && n.Content.Contains(query)))
                .OrderByDescending(n => n.IsPinned)
                .ThenByDescending(n => n.UpdatedAt)
                .Select(n => new
                {
                    noteId = n.NoteId,
                    userId = n.UserId,
                    title = n.Title,
                    content = n.Content,
                    isPinned = n.IsPinned,
                    createdAt = n.CreatedAt,
                    updatedAt = n.UpdatedAt
                })
                .ToListAsync();

            return Ok(notes);
        }
    }

    // DTOs for Note operations
    public class CreateNoteDTO
    {
        public int UserId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public bool? IsPinned { get; set; }
    }

    public class UpdateNoteDTO
    {
        public int UserId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public bool? IsPinned { get; set; }
    }
}