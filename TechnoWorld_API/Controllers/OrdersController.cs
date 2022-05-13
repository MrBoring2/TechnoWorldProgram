using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechnoWorld_API.Data;
using TechoWorld_DataModels_v2;
using TechnoWorld_API.Services;
using Microsoft.AspNetCore.SignalR;
using TechnoWorld_API.Helpers;
using Newtonsoft.Json;
using Serilog;
using TechnoWorld_API.Models;
using TechoWorld_DataModels_v2.Entities;
using TechnoWorld_API.Models.Filters;
using System.Collections;

namespace TechnoWorld_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IHubContext<TechnoWorldHub> _hubContext;
        private readonly TechnoWorldContext _context;

        public OrdersController(TechnoWorldContext context, IHubContext<TechnoWorldHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.Include(p => p.Status).ToListAsync();
        }
        [HttpGet("ForStatistics")]
        public async Task<ActionResult<IEnumerable<OrderElectronic>>> GetOrdersForStatistics(string chartParams)
        {
            var electronicsParameters = JsonConvert.DeserializeObject<ChartParams>(chartParams);

            if (electronicsParameters == null)
            {
                return BadRequest("Неккоректные параметры");
            }

            var electronics = new List<OrderElectronic>();
            foreach (var order in _context.Orders.Where(p => p.StatusId == 3).Where(p => p.DateOfRegistration >= electronicsParameters.StartDate && p.DateOfRegistration <= electronicsParameters.EndDate)
                .Include(p => p.OrderElectronics)
                .ThenInclude(p => p.Electronics)
                .ThenInclude(p => p.Type).AsEnumerable())
            {
                foreach (var orderElectronics in order.OrderElectronics)
                {
                    if (electronicsParameters.ElectronicsTypeId == 0 ? true : orderElectronics.Electronics.TypeId == electronicsParameters.ElectronicsTypeId && orderElectronics.Electronics.Type.CategoryId == electronicsParameters.CategoryId)
                    {
                        electronics.Add(orderElectronics);
                    }
                }
            }

            return electronics;
        }


        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders.Include(p => p.OrderElectronics)
                                             .ThenInclude(p => p.Electronics)
                                             .AsQueryable()
                                             .FirstOrDefaultAsync(p => p.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }
        [HttpGet("Filter")]
        public async Task<ActionResult<FilteredOrders>> GetElectronicsByFilter(string jsonFilter)
        {
            IEnumerable<Order> list = null;
            int count = 0;
            await Task.Run(() =>
            {
                OrdersFilter filter = JsonConvert.DeserializeObject<OrdersFilter>(jsonFilter);

                list = _context.Orders.Include(p => p.Status)
                                      .Include(p => p.OrderElectronics)
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
                count = list.Count();

                if (filter.CurrentPage > 1)
                {
                    list = list.Skip((filter.CurrentPage - 1) * filter.ItemsPerPage);
                }
                list = list.Take(filter.ItemsPerPage);
            });

            return new FilteredOrders(list, count);
        }
        [HttpPut("Distribute/{orderId}")]
        public async Task<ActionResult> DistributeOrder(int orderId, [FromBody] int storageId)
        {
            var order = await _context.Orders.Include(p => p.OrderElectronics).ThenInclude(p => p.Electronics).FirstOrDefaultAsync(p => p.OrderId == orderId);
            var storage = await _context.Storages.Include(p => p.ElectronicsToStorages).FirstOrDefaultAsync(p => p.StorageId == storageId);

            LogService.LodMessage($"Начинается выдача заказа {order.OrderNumber}...", LogLevel.Info);
            if (order != null && storage != null)
            {
                foreach (var electronic in order.OrderElectronics)
                {
                    if (storage.ElectronicsToStorages.FirstOrDefault(p => p.ElectronicsId == electronic.ElectronicsId) == null)
                    {
                        LogService.LodMessage($"Не хватает товара {electronic.Electronics.Model}на складе", LogLevel.Info);
                        return BadRequest($"Не хватает товара {electronic.Electronics.Model} на складе");
                    }
                    else
                    {
                        storage.ElectronicsToStorages.FirstOrDefault(p => p.ElectronicsId == electronic.ElectronicsId).Quantity -= electronic.Count;
                        LogService.LodMessage($"Выдан товар {electronic.Electronics.Model} в количествуе {electronic.Count}", LogLevel.Info);
                    }
                }
            }
            try
            {
                order.StatusId = 3;
                _context.SaveChanges();
                LogService.LodMessage($"Выдача заказа {order.OrderNumber} закончена.", LogLevel.Info);
                await _hubContext.Clients.Group(SignalRGroups.terminal_group).SendAsync("UpdateElectronics", "о");
                await _hubContext.Clients.Group(SignalRGroups.cash_group).SendAsync("UpdateOrders", "о");
                await _hubContext.Clients.Group(SignalRGroups.storage_group).SendAsync("UpdateOrders", "о");
                await _hubContext.Clients.Group(SignalRGroups.storage_group).SendAsync("UpdateElectronics", "о");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();


        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.OrderId)
            {
                return BadRequest();
            }

            var orderInDb = _context.Orders.Include(p => p.Status).FirstOrDefault(p => p.OrderId == order.OrderId);
            var lastStatus = orderInDb.Status;
            orderInDb.StatusId = order.StatusId;
            orderInDb.EmployeeId = order.EmployeeId;
            LogService.LodMessage($"Статус заказа с номером {order.OrderNumber} изменён с '{lastStatus.Name}' на '{_context.Statuses.Find(order.StatusId).Name}'", LogLevel.Info);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            try
            {
                await _hubContext.Clients.Group(SignalRGroups.cash_group).SendAsync("UpdateOrders", "d");
            }
            catch (Exception ex)
            {

            }

            return Ok();
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            order.DateOfRegistration = order.DateOfRegistration.ToLocalTime();
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            LogService.LodMessage($"Создан новый заказ с номером {order.OrderNumber}", LogLevel.Info);

            try
            {
                await _hubContext.Clients.Group(SignalRGroups.cash_group).SendAsync("UpdateOrders", "d");
            }
            catch (Exception ex)
            {

            }

            return CreatedAtAction("GetOrder", new { id = order.OrderId }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
