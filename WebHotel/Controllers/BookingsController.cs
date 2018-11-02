using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebHotel.Data;
using Microsoft.AspNetCore.Authorization;
using WebHotel.Models;

namespace WebHotel.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }


        [Authorize(Roles ="Customers")]
        public IActionResult BookARoom()
        {
            
            ViewData["RoomID"] = new SelectList(_context.Room, "ID", "ID");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookARoom(ConfirmBookingModel confirm)
        {
            if (ModelState.IsValid)
            {
                var book = new Booking
                {
                    RoomID = confirm.RoomID,
                    CustomerEmail = User.FindFirst(ClaimTypes.Name).Value,
                    CheckIn = confirm.CheckIn,
                    CheckOut = confirm.CheckOut
                };

                var theRooom = await _context.Room.FindAsync(confirm.RoomID);
                book.Cost = theRooom.Price * (book.CheckOut - book.CheckIn).Days;

                _context.Add(book);
                await _context.SaveChangesAsync();

                ViewBag.cost = book.Cost;
                ViewBag.level = book.TheRoom.Level;
                //return RedirectToAction(nameof(CIndex));
            }
            
            ViewData["RoomID"] = new SelectList(_context.Room, "ID", "ID", confirm.RoomID);
            return View(confirm);
        }


        [Authorize(Roles ="Customers")]
        public async Task<IActionResult> CIndex(string sortOrder)
        {
            //var applicationDbContext = _context.Booking.Include(b => b.TheCustomer).Include(b => b.TheRoom);
            var book = (IQueryable<Booking>)_context.Booking.Include(x => x.TheCustomer).Include(x => x.TheRoom);

            switch (sortOrder)
            {
                case "checkin_asc":
                    book = book.OrderBy(m => m.CheckIn);
                    break;
                case "checkin_desc":
                    book = book.OrderByDescending(m => m.CheckIn);
                    break;
                case "cost_asc":
                    book = book.OrderBy(m => m.Cost);
                    break;
                case "cost_desc":
                    book = book.OrderByDescending(m => m.Cost);
                    break;

            }

            ViewData["NextCheckinOrder"] = sortOrder != "checkin_asc" ? "checkin_asc" : "checkin_desc";
            ViewData["NextCostOrder"] = sortOrder != "cost_asc" ? "cost_asc" : "cost_desc";

            ViewBag.User = User.FindFirst(ClaimTypes.Name).Value;
            return View(await book.AsNoTracking().ToListAsync());
            //return View(await applicationDbContext.ToListAsync());
        }

        // GET: Bookings
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Booking.Include(b => b.TheCustomer).Include(b => b.TheRoom);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.TheCustomer)
                .Include(b => b.TheRoom)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["CustomerEmail"] = new SelectList(_context.Customer, "Email", "Email");
            ViewData["RoomID"] = new SelectList(_context.Room, "ID", "ID");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,RoomID,CustomerEmail,CheckIn,CheckOut,Cost")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerEmail"] = new SelectList(_context.Customer, "Email", "Email", booking.CustomerEmail);
            ViewData["RoomID"] = new SelectList(_context.Room, "ID", "ID", booking.RoomID);
            return View(booking);
        }

        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Stats(CustomerStats s)
        {
            var postGroups = _context.Customer.GroupBy(m => m.Postcode);
            var pStats = postGroups.Select(g => new CustomerStats { PostC = g.Key, PCCount = g.Count() });
            ViewBag.cinfo = await pStats.ToListAsync();
            var roomGroups = _context.Booking.GroupBy(m => m.RoomID);
            var rStats = roomGroups.Select(g => new CustomerStats { roomID = g.Key, roomCount = g.Count() });
            ViewBag.binfo = await rStats.ToListAsync();
            return View(s);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.SingleOrDefaultAsync(m => m.ID == id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["CustomerEmail"] = new SelectList(_context.Customer, "Email", "Email", booking.CustomerEmail);
            ViewData["RoomID"] = new SelectList(_context.Room, "ID", "ID", booking.RoomID);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,RoomID,CustomerEmail,CheckIn,CheckOut,Cost")] Booking booking)
        {
            if (id != booking.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.ID))
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
            ViewData["CustomerEmail"] = new SelectList(_context.Customer, "Email", "Email", booking.CustomerEmail);
            ViewData["RoomID"] = new SelectList(_context.Room, "ID", "ID", booking.RoomID);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.TheCustomer)
                .Include(b => b.TheRoom)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.SingleOrDefaultAsync(m => m.ID == id);
            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.ID == id);
        }
    }
}
