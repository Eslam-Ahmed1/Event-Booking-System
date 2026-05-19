using EventBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EventBookingSystem.Controllers
{
    [Authorize(Roles = "admin")]
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Event
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.OrderByDescending(e => e.date).ToListAsync());
        }

        // GET: Event/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.event_id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Event/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("title,date,location,total_seats,image_url")] Event @event)
        {
            if (ModelState.IsValid)
            {
                // When an event is created, the number of available seats is the same as the total seats.
                @event.available_seats = @event.total_seats;
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Event/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Event/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("event_id,title,date,location,total_seats,available_seats,image_url")] Event @event)
        {
            if (id != @event.event_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Preserve the original creation date, which should not be editable.
                    var originalEvent = await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.event_id == id);
                    if (originalEvent != null) {
                        @event.created_at = originalEvent.created_at;
                    }
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.event_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Event/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FirstOrDefaultAsync(m => m.event_id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                var hasBookings = await _context.Bookings.AnyAsync(b => b.event_id == id);
                if (hasBookings)
                {
                    TempData["ErrorMessage"] = "This event cannot be deleted because it has existing bookings.";
                    return RedirectToAction(nameof(Index));
                }
                _context.Events.Remove(@event);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.event_id == id);
        }
    }
}