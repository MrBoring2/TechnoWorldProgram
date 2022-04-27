using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BNS_API.Data;
using TechoWorld_DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TechnoWorld_API.Services;
using Serilog;
using TechnoWorld_API.Helpers;
using Newtonsoft.Json;

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
        // GET: api/Electronics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Electronic>>> GetElectronicsByCategory(int categoryId)
        {
            var list = await _context.Electronics.Include(p => p.Manufacturer).Include(p => p.Type).Include(p => p.ElectronicsToStorages).Where(p => p.Type.CategoryId == categoryId).Where(p => p.IsOfferedForSale == true).ToListAsync();

            foreach (var item in list)
            {
                foreach (var storage in item.ElectronicsToStorages)
                {
                    _context.Entry(storage).Reference(p => p.Storage).Load();
                }
            }
            return list;
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
            Log.Information($"Изменён товар {electronic.Model}");

            var allElectronics = await _context.Electronics.Include(p => p.Type).Include(p => p.Type.Category).ToListAsync();
            foreach (var item in allElectronics)
            {
                foreach (var storage in item.ElectronicsToStorages)
                {
                    await _context.Entry(storage).Reference(p => p.Storage).LoadAsync();
                }
            }
            await _hubContext.Clients.Group(SignalRGroups.storage_group).SendAsync("UpdateElectronics", JsonConvert.SerializeObject(allElectronics, Formatting.None,
                    new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));

            var categoryElectronics = await _context.Electronics.Include(p => p.Type).Include(p => p.Type.Category).Where(p => p.Type.CategoryId == electronic.Type.CategoryId).ToListAsync();
            foreach (var item in categoryElectronics)
            {
                foreach (var storage in item.ElectronicsToStorages)
                {
                    await _context.Entry(storage).Reference(p => p.Storage).LoadAsync();
                }
            }
            await _hubContext.Clients.Group(SignalRGroups.terminal_group).SendAsync("UpdateElectronics", JsonConvert.SerializeObject(categoryElectronics, Formatting.None,
                    new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));

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

            Log.Information($"Добавлен товар {electronic.Model}");
            var allElectronics = await _context.Electronics.Include(p => p.Type).Include(p => p.Type.Category).ToListAsync();
            foreach (var item in allElectronics)
            {
                foreach (var storage in item.ElectronicsToStorages)
                {
                    await _context.Entry(storage).Reference(p => p.Storage).LoadAsync();
                }
            }
            await _hubContext.Clients.Group(SignalRGroups.storage_group).SendAsync("UpdateElectronics", JsonConvert.SerializeObject(allElectronics, Formatting.None,
                    new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));

            var categoryElectronics = await _context.Electronics.Include(p => p.Type).Include(p => p.Type.Category).Where(p => p.Type.CategoryId == electronic.Type.CategoryId).ToListAsync();
            foreach (var item in categoryElectronics)
            {
                foreach (var storage in item.ElectronicsToStorages)
                {
                    await _context.Entry(storage).Reference(p => p.Storage).LoadAsync();
                }
            }
            await _hubContext.Clients.Group(SignalRGroups.terminal_group).SendAsync("UpdateElectronics", JsonConvert.SerializeObject(categoryElectronics, Formatting.None,
                    new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));

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
