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
using TechnoWorld_API.Services;
using Microsoft.AspNetCore.SignalR;
using TechnoWorld_API.Helpers;
using Serilog;
using TechoWorld_DataModels_v2.Entities;

namespace TechnoWorld_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly IHubContext<TechnoWorldHub> _hubContext;
        private readonly TechnoWorldContext _context;
        public CategoriesController(TechnoWorldContext context, IHubContext<TechnoWorldHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
            
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            var dbCategory = await _context.Manufacturers.FirstOrDefaultAsync(p => p.Name.Equals(category.Name));
            if (dbCategory != null)
            {
                return BadRequest("Категория с таким названием уже существует!");
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.Group(SignalRGroups.terminal_group).SendAsync("UpdateCategories", category.Id);

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
