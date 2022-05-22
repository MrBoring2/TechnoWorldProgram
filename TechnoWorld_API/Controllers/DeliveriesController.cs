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
using TechnoWorld_API.Models.Filters;
using TechnoWorld_API.Services;
using TechoWorld_DataModels_v2;
using TechoWorld_DataModels_v2.Entities;

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
                                                .ThenInclude(p => p.Electronics)
                                                .AsSplitQuery()
                                                .AsNoTracking()
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
                var l = list.ToList();
                count = list.Count();
                if (filter.CurrentPage > 1)
                {
                    list = list.Skip((filter.CurrentPage - 1) * filter.ItemsPerPage);
                }
                list = list.Take(filter.ItemsPerPage);
            });

            return Ok(new FilteredDeliveries(list, count));
        }
        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Delivery>>> GetDeliveries()
        {
            return Ok(_context.Deliveries.Include(p => p.Status)
                                                .Include(p => p.Storage)
                                                .Include(p => p.ElectronicsToDeliveries)
                                                .ThenInclude(p => p.Electronics)
                                                .AsEnumerable());
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
            delivery.DateOfDelivery = delivery.DateOfDelivery.ToLocalTime();
            delivery.DateOfOrder = delivery.DateOfOrder.ToLocalTime();
            var deliveryInDb = await _context.Deliveries.Include(p => p.Status).FirstOrDefaultAsync(p => p.DelivertId == id);
            var lastStatus = deliveryInDb.Status;

            _context.Entry(deliveryInDb).CurrentValues.SetValues(delivery);
            try
            {
                await _context.SaveChangesAsync();
                LogService.LodMessage($"Статус поставки с номером {deliveryInDb.DeliveryNumber} изменён с '{lastStatus.Name}' на '{_context.Statuses.Find(deliveryInDb.StatusId).Name}'", LogLevel.Info);
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

            await _hubContext.Clients.Group(SignalRGroups.storage_group).SendAsync("UpdateDeliveries", "d");

            return Ok();
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostDelivery(Delivery delivery)
        {
            delivery.DateOfDelivery = delivery.DateOfDelivery.ToLocalTime();
            delivery.DateOfOrder = delivery.DateOfOrder.ToLocalTime();
            _context.Entry(delivery).State = EntityState.Added;
            foreach (var item in delivery.ElectronicsToDeliveries)
            {
                _context.Entry(item).State = EntityState.Added;
            }

            _context.Deliveries.Add(delivery);
            try
            {
                await _context.SaveChangesAsync();
                LogService.LodMessage($"Создан заказ на поставку с номером {delivery.DeliveryNumber}", LogLevel.Info);
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            await _hubContext.Clients.Group(SignalRGroups.storage_group).SendAsync("UpdateDeliveries", "d");

            return CreatedAtAction("GetDelivery", new { id = delivery.DelivertId }, delivery);
        }
        [HttpPut("Unload/{deliveryId}")]
        public async Task<ActionResult> UnloadDelivery(int deliveryId)
        {
            var delivery = await _context.Deliveries.Include(p => p.Storage).Include(p => p.ElectronicsToDeliveries).FirstOrDefaultAsync(p => p.DelivertId == deliveryId);
            if (delivery == null)
            {
                return NotFound();
            }

            var storage = delivery.Storage;
            await _context.Entry(storage).Collection(p => p.ElectronicsToStorages).LoadAsync();

            LogService.LodMessage($"Происходит выгрузка товара на склад '{storage.Name}'...", LogLevel.Info);
            foreach (var item in delivery.ElectronicsToDeliveries)
            {
                if (storage.ElectronicsToStorages.FirstOrDefault(p => p.ElectronicsId == item.ElectronicsId) == null)
                {
                    await _context.Entry(item).Reference(p => p.Electronics).LoadAsync();
                    storage.ElectronicsToStorages.Add(new ElectronicsToStorage { ElectronicsId = item.ElectronicsId, StorageId = storage.StorageId, Quantity = item.Quantity });
                    LogService.LodMessage($"Выгрузка товара {item.Electronics.Model}, текущее количество: {item.Quantity}", LogLevel.Info);
                }
                else
                {
                    await _context.Entry(item).Reference(p => p.Electronics).LoadAsync();
                    var itemInStorage = storage.ElectronicsToStorages.FirstOrDefault(p => p.ElectronicsId == item.ElectronicsId);
                    itemInStorage.Quantity += item.Quantity;
                    LogService.LodMessage($"Выгрузка товара {item.Electronics.Model}, текущее количество: {itemInStorage.Quantity}", LogLevel.Info);
                }
            }

            try
            {
                delivery.StatusId = 3;
                await _context.SaveChangesAsync();
                LogService.LodMessage($"Выгрузка товара завершена.", LogLevel.Info);
            }
            catch (DbUpdateConcurrencyException)
            {
                LogService.LodMessage($"Произошла ошибка при выгрузке товара...", LogLevel.Error);
                return BadRequest("Произошла ошибка при выгрузке товара...");
            }


            await _hubContext.Clients.Group(SignalRGroups.storage_group).SendAsync("UpdateDeliveries", "d");
            await _hubContext.Clients.Group(SignalRGroups.storage_group).SendAsync("UpdateElectronics", "d");
            await _hubContext.Clients.Group(SignalRGroups.terminal_group).SendAsync("UpdateElectronics", "d");

            return Ok();
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
