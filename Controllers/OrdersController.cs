using EventBriteClone.Data;
using EventBriteClone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBriteClone.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var order = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Ticket)
                        .ThenInclude(t => t.Event)
                .FirstOrDefault(o => o.Id == id && o.UserId == userId.Value);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        public IActionResult Create(int eventId, int ticketId, int quantity)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var ticket = _context.Tickets
                .Include(t => t.Event)
                .FirstOrDefault(t => t.Id == ticketId && t.EventId == eventId);

            if (ticket == null || ticket.QuantitySold + quantity > ticket.Quantity)
            {
                return BadRequest("Tickets not available");
            }

            var order = new Order
            {
                UserId = userId.Value,
                OrderNumber = GenerateOrderNumber(),
                TotalAmount = ticket.Price * quantity,
                Status = "Completed",
                OrderDate = DateTime.Now,
                PaymentStatus = "Paid"
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                TicketId = ticketId,
                Quantity = quantity,
                Price = ticket.Price,
                TicketCode = GenerateTicketCode()
            };

            _context.OrderItems.Add(orderItem);

            ticket.QuantitySold += quantity;
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = order.Id });
        }

        private string GenerateOrderNumber()
        {
            return "ORD-" + DateTime.Now.Ticks.ToString().Substring(8);
        }

        private string GenerateTicketCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();
        }
    }
}
