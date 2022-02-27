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
    public class ElectrnicsTypesController : ControllerBase
    {
        private readonly TechnoWorldContext _context;

        public ElectrnicsTypesController(TechnoWorldContext context)
        {
            _context = context;
        }

        // GET: api/ElectrnicsTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ElectrnicsType>>> GetElectrnicsTypes(int categoryId)
        {
            return await _context.ElectrnicsTypes.Where(p => p.Category.Id == categoryId).ToListAsync();
        }

        // GET: api/ElectrnicsTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ElectrnicsType>> GetElectrnicsType(int id)
        {
            var electrnicsType = await _context.ElectrnicsTypes.FindAsync(id);

            if (electrnicsType == null)
            {
                return NotFound();
            }

            return electrnicsType;
        }

        // PUT: api/ElectrnicsTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutElectrnicsType(int id, ElectrnicsType electrnicsType)
        {
            if (id != electrnicsType.TypeId)
            {
                return BadRequest();
            }

            _context.Entry(electrnicsType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ElectrnicsTypeExists(id))
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

        // POST: api/ElectrnicsTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ElectrnicsType>> PostElectrnicsType(ElectrnicsType electrnicsType)
        {
            _context.ElectrnicsTypes.Add(electrnicsType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetElectrnicsType", new { id = electrnicsType.TypeId }, electrnicsType);
        }

        // DELETE: api/ElectrnicsTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteElectrnicsType(int id)
        {
            var electrnicsType = await _context.ElectrnicsTypes.FindAsync(id);
            if (electrnicsType == null)
            {
                return NotFound();
            }

            _context.ElectrnicsTypes.Remove(electrnicsType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ElectrnicsTypeExists(int id)
        {
            return _context.ElectrnicsTypes.Any(e => e.TypeId == id);
        }
    }
}
