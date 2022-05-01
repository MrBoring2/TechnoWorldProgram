using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechnoWorld_API.Data;
using TechnoWorld_API.Helpers;
using TechnoWorld_API.Models;
using TechnoWorld_API.Services;
using TechoWorld_DataModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TechnoWorld_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveriesController : ControllerBase
    {
        private readonly IHubContext<TechnoWorldHub> _hubContext;
        private readonly TechnoWorldContext _context;
        public DeliveriesController(TechnoWorldContext context, IHubContext<TechnoWorldHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;

        }
        // GET: api/Categories
        [HttpGet("Filter")]
        public async Task<ActionResult<FilteredDeliveries>> GetDeliveriesWithFilter(string jsonFilter)
        {
            IEnumerable<Delivery> list = null;
            int count = 0;
            await Task.Run(() =>
            {
                DeliveriesFilter filter = JsonConvert.DeserializeObject<DeliveriesFilter>(jsonFilter);

                list = _context.Deliveries.Include(p => p.Status)
                                                .Include(p => p.Storage)
                                                .Include(p => p.ElectronicsToDeliveries)
                                                .AsSplitQuery()
                                                //.AsNoTracking()
                                                .Where(filter.FilterExpression)
                                                .AsEnumerable();

                if (filter.IsAscending)
                {
                    list = list.OrderBy(p => p.GetProperty(filter.SortParameter));
                }
                else
                {
                    list = list.OrderByDescending(p => p.GetProperty(filter.SortParameter));
                }
                count = list.Count();
                list = list.ToList();
                if (filter.CurrentPage > 1)
                {
                    list = list.Skip((filter.CurrentPage - 1) * filter.ItemsPerPage);
                }
                list = list.Take(filter.ItemsPerPage);
                foreach (var item in list)
                {
                    foreach (var storage in item.ElectronicsToDeliveries)
                    {
                        _context.Entry(storage).Reference(p => p.Electronics).Load();
                    }
                }
            });

            return new FilteredDeliveries(list, count);
        }
        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Delivery>>> GetDeliveries()
        {
            var list = await _context.Deliveries.Include(p => p.Status).Include(p => p.Storage).Include(p => p.ElectronicsToDeliveries).ToListAsync();
            foreach (var item in list)
            {
                foreach (var electronic in item.ElectronicsToDeliveries)
                {
                    await _context.Entry(electronic).Reference(p => p.Electronics).LoadAsync();
                }
            }
            return list;
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Delivery>> GetDelivery(int id)
        {
            var delivery = await _context.Deliveries.FindAsync(id);

            if (delivery == null)
            {
                return NotFound();
            }

            return delivery;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDelivery(int id, Delivery delivery)
        {
            if (id != delivery.DelivertId)
            {
                return BadRequest();
            }

            var deliveryInDb = _context.Deliveries.Include(p => p.Status).FirstOrDefault(p => p.DelivertId == id);

            _context.Entry(deliveryInDb).CurrentValues.SetValues(delivery);

            var lastStatus = deliveryInDb.Status;
            deliveryInDb.StatusId = delivery.StatusId;
            deliveryInDb.EmployeeId = delivery.EmployeeId;

            try
            {
                await _context.SaveChangesAsync();
                Log.Information($"Статус заказа с номером {delivery.DeliveryNumber} изменён с '{lastStatus.Name}' на '{_context.Statuses.Find(delivery.StatusId).Name}'");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeliveryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var list = await _context.Deliveries.Include(p => p.Status).Include(p => p.Storage).Include(p => p.ElectronicsToDeliveries).ToListAsync();
            foreach (var item in list)
            {
                foreach (var electronic in item.ElectronicsToDeliveries)
                {
                    await _context.Entry(electronic).Reference(p => p.Electronics).LoadAsync();
                }
            }
            await _hubContext.Clients.Group(SignalRGroups.storage_group).SendAsync("UpdateDeliveries", JsonConvert.SerializeObject(list, Formatting.None,
                new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));

            return NoContent();
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostDelivery(Delivery delivery)
        {
            _context.Deliveries.Add(delivery);

            Log.Information($"Создан заказ на поставку с номером {delivery.DeliveryNumber}");
            await _context.SaveChangesAsync();

            var list = await _context.Deliveries.Include(p => p.Status).Include(p => p.Storage).Include(p => p.ElectronicsToDeliveries).ToListAsync();
            foreach (var item in list)
            {
                foreach (var electronic in item.ElectronicsToDeliveries)
                {
                    await _context.Entry(electronic).Reference(p => p.Electronics).LoadAsync();
                }
            }
            await _hubContext.Clients.Group(SignalRGroups.storage_group).SendAsync("UpdateDeliveries", JsonConvert.SerializeObject(list, Formatting.None,
                new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));

            return CreatedAtAction("GetDelivery", new { id = delivery.DelivertId }, delivery);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDelivery(int id)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery == null)
            {
                return NotFound();
            }

            _context.Deliveries.Remove(delivery);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DeliveryExists(int id)
        {
            return _context.Deliveries.Any(e => e.DelivertId == id);
        }
    }
}
