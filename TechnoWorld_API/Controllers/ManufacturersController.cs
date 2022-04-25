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

namespace BNS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ManufacturersController : ControllerBase
    {
        private readonly TechnoWorldContext _context;

        public ManufacturersController(TechnoWorldContext context)
        {
            _context = context;
        }
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<Manufacturer>>> GetAllManufacturers()
        {
            return _context.Manufacturers.ToList();
        }
        // GET: api/Manufacturers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Manufacturer>>> GetManufacturers(int categoryId)
        {
            var manufacturers = new List<Manufacturer>();
            await Task.Run(() =>
            {
                foreach (var item in _context.Manufacturers.Include(p => p.Electronics))
                {
                    foreach (var electronics in item.Electronics)
                    {
                        _context.Entry(electronics).Reference(p => p.Type).Load();
                    }
                    if (item.Electronics.Any(p => p.Type.CategoryId == categoryId))
                    {
                        manufacturers.Add(item);
                    }
                }
            });
            return manufacturers;
        }

        // GET: api/Manufacturers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Manufacturer>> GetManufacturer(int id)
        {
            var manufacturer = await _context.Manufacturers.FindAsync(id);

            if (manufacturer == null)
            {
                return NotFound();
            }

            return manufacturer;
        }

        // PUT: api/Manufacturers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutManufacturer(int id, Manufacturer manufacturer)
        {
            if (id != manufacturer.ManufacturerId)
            {
                return BadRequest();
            }

            _context.Entry(manufacturer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ManufacturerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Manufacturers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Manufacturer>> PostManufacturer(Manufacturer manufacturer)
        {
            _context.Manufacturers.Add(manufacturer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetManufacturer", new { id = manufacturer.ManufacturerId }, manufacturer);
        }

        // DELETE: api/Manufacturers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteManufacturer(int id)
        {
            var manufacturer = await _context.Manufacturers.FindAsync(id);
            if (manufacturer == null)
            {
                return NotFound();
            }

            _context.Manufacturers.Remove(manufacturer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ManufacturerExists(int id)
        {
            return _context.Manufacturers.Any(e => e.ManufacturerId == id);
        }
    }
}
