using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechnoWorld_API.Data;
using TechoWorld_DataModels_v2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TechnoWorld_API.Services;
using Serilog;
using TechnoWorld_API.Helpers;
using Newtonsoft.Json;
using TechnoWorld_API.Models;
using TechoWorld_DataModels_v2.Entities;
using TechnoWorld_API.Models.Filters;

namespace BNS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ElectronicsController : ControllerBase
    {
        private readonly IHubContext<TechnoWorldHub> _hubContext;
        private readonly TechnoWorldContext _context;
        public ElectronicsController(TechnoWorldContext context, IHubContext<TechnoWorldHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        // GET: api/Electronics
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<Electronic>>> GetAllElectronics()
        {
            var list = await _context.Electronics.Include(p => p.Manufacturer).Include(p => p.ElectronicsToStorages).Include(p => p.Type).Include(p => p.Type.Category).ToListAsync();

            foreach (var item in list)
            {
                foreach (var storage in item.ElectronicsToStorages)
                {
                    await _context.Entry(storage).Reference(p => p.Storage).LoadAsync();
                }
            }

            return list;
        }

        [HttpGet("TerminalFilter")]
        public async Task<ActionResult<FilteredElectronic>> GetElectronicsForTerminalByFilter(string jsonFilter)
        {
            IEnumerable<Electronic> list = null;
            int count = 0;
            await Task.Run(() =>
            {
                ElectronicsTerminalFilter filter = JsonConvert.DeserializeObject<ElectronicsTerminalFilter>(jsonFilter);

                list = _context.Electronics.Include(p => p.Type)
                                                .Include(p => p.Type.Category)
                                                .Include(p => p.Manufacturer)
                                                .Include(p => p.ElectronicsToStorages)
                                                .ThenInclude(p => p.Storage)
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
                list = list.ToList();
                if (filter.CurrentPage > 1)
                {
                    list = list.Skip((filter.CurrentPage - 1) * filter.ItemsPerPage);
                }
                list = list.Take(filter.ItemsPerPage);
            });

            return new FilteredElectronic(list, count);
        }


        [HttpGet("Filter")]
        public async Task<ActionResult<FilteredElectronic>> GetElectronicsByFilter(string jsonFilter)
        {

            return await Task.Run(() =>
            {
                IEnumerable<Electronic> list = null;
                int count = 0;
                ElectronicsFilter filter = JsonConvert.DeserializeObject<ElectronicsFilter>(jsonFilter);

                list = _context.Electronics.Include(p => p.Type)
                                                .Include(p => p.Type.Category)
                                                .Include(p => p.Manufacturer)
                                                .Include(p => p.ElectronicsToStorages)
                                                .ThenInclude(p => p.Storage)
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
                return new FilteredElectronic(list, count);
            });


        }
        // GET: api/Electronics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Electronic>>> GetElectronicsByCategory(int categoryId)
        {
            return Ok(_context.Electronics.Include(p => p.Manufacturer)
                                                 .Include(p => p.Type)
                                                 .Include(p => p.ElectronicsToStorages)
                                                 .ThenInclude(p => p.Storage)
                                                 .Where(p => p.Type.CategoryId == categoryId)
                                                 .Where(p => p.IsOfferedForSale == true)
                                                 .AsEnumerable());
        }

        // GET: api/Electronics/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Electronic>> GetElectronic(int id)
        {
            var electronic = await _context.Electronics.FindAsync(id);

            if (electronic == null)
            {
                return NotFound();
            }

            return electronic;
        }

        // PUT: api/Electronics/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutElectronic(int id, Electronic electronic)
        {
            if (id != electronic.ElectronicsId)
            {
                return BadRequest();
            }

            _context.Entry(electronic).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ElectronicExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            LogService.LodMessage($"Изменён товар {electronic.Model}", LogLevel.Info);

            await _hubContext.Clients.Group(SignalRGroups.storage_group).SendAsync("UpdateElectronics", "о");

            await _hubContext.Clients.Group(SignalRGroups.terminal_group).SendAsync("UpdateElectronics", "о");

            return NoContent();
        }

        // POST: api/Electronics
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Electronic>> PostElectronic(Electronic electronic)
        {
            _context.Entry(electronic).State = EntityState.Added;
            _context.Electronics.Add(electronic);
            await _context.SaveChangesAsync();

            LogService.LodMessage($"Добавлен товар {electronic.Model}", LogLevel.Info);

            await _hubContext.Clients.Group(SignalRGroups.storage_group).SendAsync("UpdateElectronics", "о");

            await _hubContext.Clients.Group(SignalRGroups.terminal_group).SendAsync("UpdateElectronics", "о");
            return CreatedAtAction("GetElectronic", new { id = electronic.ElectronicsId }, electronic);
        }

        // DELETE: api/Electronics/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteElectronic(int id)
        {
            var electronic = await _context.Electronics.FindAsync(id);
            if (electronic == null)
            {
                return NotFound();
            }

            _context.Electronics.Remove(electronic);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ElectronicExists(int id)
        {
            return _context.Electronics.Any(e => e.ElectronicsId == id);
        }
    }
}
