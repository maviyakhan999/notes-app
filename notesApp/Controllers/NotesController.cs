using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using notesApp.Models;

namespace notesApp.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Notes/GetAll
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var userId = User.Identity.GetUserId();
            var notes = await db.Notes
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.UpdatedAt)
                .Select(n => new
                {
                    n.Id,
                    n.Title,
                    n.Content,
                    n.CreatedAt,
                    n.UpdatedAt
                })
                .ToListAsync();

            return Json(notes, JsonRequestBehavior.AllowGet);
        }

        // POST: /Notes/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(int? id, string title, string content)
        {
            var userId = User.Identity.GetUserId();

            if (string.IsNullOrWhiteSpace(title))
            {
                title = "Untitled Note";
            }

            Note note;
            if (id.HasValue && id.Value > 0)
            {
                // Update existing note
                note = await db.Notes.FirstOrDefaultAsync(n => n.Id == id.Value && n.UserId == userId);
                if (note == null)
                {
                    return Json(new { success = false, message = "Note not found" });
                }

                note.Title = title;
                note.Content = content ?? "";
                note.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Create new note
                note = new Note
                {
                    Title = title,
                    Content = content ?? "",
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                db.Notes.Add(note);
            }

            await db.SaveChangesAsync();

            return Json(new
            {
                success = true,
                note = new
                {
                    note.Id,
                    note.Title,
                    note.Content,
                    note.CreatedAt,
                    note.UpdatedAt
                }
            });
        }

        // POST: /Notes/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = User.Identity.GetUserId();
            var note = await db.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null)
            {
                return Json(new { success = false, message = "Note not found" });
            }

            db.Notes.Remove(note);
            await db.SaveChangesAsync();

            return Json(new { success = true });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
