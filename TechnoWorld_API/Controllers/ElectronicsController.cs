﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BNS_API.Data;

namespace BNS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElectronicsController : ControllerBase
    {
        private readonly BNSContext _context;

        public ElectronicsController(BNSContext context)
        {
            _context = context;
        }

        // GET: api/Electronics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Electronic>>> GetElectronics()
        {
            return await _context.Electronics.Include(p => p.Manufactrurer).Include(p => p.Type).Include(p => p.ElectronicsToStorages).ToListAsync();
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

            return NoContent();
        }

        // POST: api/Electronics
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Electronic>> PostElectronic(Electronic electronic)
        {
            _context.Electronics.Add(electronic);
            await _context.SaveChangesAsync();

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
