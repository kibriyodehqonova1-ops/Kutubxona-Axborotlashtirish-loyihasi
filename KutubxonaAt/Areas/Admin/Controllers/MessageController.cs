using KutubxonaAt.Data;
using KutubxonaAt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KutubxonaAt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MessageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Message
        // GET: Admin/Message
        public async Task<IActionResult> Index(bool unreadOnly = false)
        {
            IQueryable<ContactMessage> query = _context.ContactMessages
                .Include(m => m.User)
                .OrderByDescending(m => m.YuborilganSana); // IOrderedQueryable is assignable to IQueryable

            if (unreadOnly)
                query = query.Where(m => !m.Oqilgan); // Now it's IQueryable, assignable

            var messages = await query.ToListAsync();
            ViewBag.UnreadOnly = unreadOnly;
            return View(messages);
        }
        // GET: Admin/Message/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var message = await _context.ContactMessages
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null) return NotFound();

            if (!message.Oqilgan)
            {
                message.Oqilgan = true;
                message.OqilganSana = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return View(message);
        }

        // POST: Admin/Message/Reply
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(int id, string replyText)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message == null) return NotFound();

            message.AdminJavobi = replyText;
            message.JavobYozilganSana = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Javob saqlandi.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Admin/Message/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message == null) return NotFound();
            _context.ContactMessages.Remove(message);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xabar o‘chirildi.";
            return RedirectToAction(nameof(Index));
        }
    }
}