using EventBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EventBookingSystem.Controllers
{

    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookingController> _logger;
        public BookingController(ApplicationDbContext context, ILogger<BookingController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Booking/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("login", "Account");
            }

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("login", "Account");
            }

            var userBookings = await _context.Bookings
                .Include(b => b.Event) // Include the related Event data
                .Where(b => b.user_id == userId)
                .OrderByDescending(b => b.booking_date)
                .ToListAsync();

            return View(userBookings);
        }

        // GET: /Booking/Create/5
        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("login", "Account");
            }

            var eventToBook = await _context.Events.FindAsync(id);
            if (eventToBook == null)
            {
                return NotFound();
            }

            // Pass the event to the view to display its details.
            return View(eventToBook);
        }

        // POST: /Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, int numberOfTickets)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("login", "Account");
            }

            // 1. Get the current user's ID from the JWT claims.
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("login", "Account");
            }

            var eventToBook = await _context.Events.FindAsync(id);
            if (eventToBook == null)
            {
                return NotFound();
            }

            // 2. Validate the booking request.
            if (numberOfTickets <= 0)
            {
                ModelState.AddModelError(string.Empty, "You must book at least one ticket.");
            }
            if (eventToBook.available_seats < numberOfTickets)
            {
                ModelState.AddModelError(string.Empty, $"Sorry, only {eventToBook.available_seats} seats are available for this event.");
            }

            if (!ModelState.IsValid)
            {
                return View(eventToBook); // Return to the view with validation errors.
            }

            // 3. Create the booking and update the event seat count.
            var booking = new Booking
            {
                event_id = id,
                user_id = userId,
                number_of_tickets = numberOfTickets,
                booking_date = DateTime.UtcNow
            };

            eventToBook.available_seats -= numberOfTickets;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Successfully booked {numberOfTickets} ticket(s) for {eventToBook.title}!";
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int bookingId)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("login", "Account");
            }
            _logger.LogInformation($"Cancel method called with id: {bookingId}");
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
            {
                return NotFound();
            }   
            else
            {
                var eventItem = await _context.Events.FindAsync(booking.event_id);
                if (eventItem == null){
                    return NotFound();
                }
                else{
                    eventItem.available_seats += booking.number_of_tickets;
                    _context.Bookings.Remove(booking);
                    await _context.SaveChangesAsync();
                }
            return RedirectToAction("Index", "Booking");
            }
        
        }
    }
}