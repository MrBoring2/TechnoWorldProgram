using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TechnoWorld_API.Data;
using TechnoWorld_API.Helpers;
using TechnoWorld_API.Models.Filters;
using TechnoWorld_API.Services;
using TechoWorld_DataModels_v2;
using TechoWorld_DataModels_v2.Entities;

namespace TechnoWorld_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IHubContext<TechnoWorldHub> _hubContext;
        private readonly TechnoWorldContext _context;

        public EmployeesController(TechnoWorldContext context, IHubContext<TechnoWorldHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            return await _context.Employees.ToListAsync();
        }
        [HttpGet("Filter")]
        public async Task<ActionResult<FilteredEmployees>> GetEmployeesByFilter(string jsonFilter)
        {

            return await Task.Run(() =>
            {
                IEnumerable<Employee> list = null;
                int count = 0;
                EmployeesFilter filter = JsonConvert.DeserializeObject<EmployeesFilter>(jsonFilter);

                list = _context.Employees.Include(p => p.Post)
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
                return new FilteredEmployees(list, count);
            });


        }
        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return BadRequest();
            }

            var employeeInDb = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(p => p.EmployeeId == id);

            if (TechnoWorldHub.ConnectedUsers.Any(p => p.Key == employeeInDb.Login))
            {
                LogService.LodMessage($"Провальная попытка изменения пользователя {employeeInDb.Login}: пользователь сейчас онлайн в системе.", LogLevel.Error);
                return BadRequest("Данный пользователь в данный момент онлайн в системе, изменение невозможно.");
            }

            var user = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(p => p.EmployeeId == id);

            if (user.Login != employee.Login)
            {
                if (_context.Employees.FirstOrDefault(p => p.Login == employee.Login) != null)
                {
                    LogService.LodMessage($"Провальная попытка изменения пользователя {employeeInDb.Login}: пользователь с логином {employee.Login} уже существует.", LogLevel.Error);
                    return BadRequest($"Пользователь с логином {employee.Login} уже существует");
                }
            }
            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                LogService.LodMessage($"Было провизведено изменение пользователя {employee.Login} с пролью {employee.Post.Name}", LogLevel.Info);
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            await _hubContext.Clients.Group(SignalRGroups.storage_group).SendAsync("UpdateEmployees", "d");

            return Ok();
        }

        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            if (_context.Employees.FirstOrDefault(p => p.Login == employee.Login) != null)
            {
                LogService.LodMessage($"Провальная попытка добавления нового пользователя {employee.Login}: пользователь с логином {employee.Login} уже существует.", LogLevel.Error);
                return BadRequest($"Пользователь с логином {employee.Login} уже существует");
            }
            _context.Employees.Attach(employee).State = EntityState.Added;
            //_context.Employees.Add(employee);UpdateEmployees
            await _context.SaveChangesAsync();
            await _hubContext.Clients.Group(SignalRGroups.storage_group).SendAsync("UpdateEmployees", "d");
            LogService.LodMessage($"Добавлен новый пользователь {employee.Login} с ролью {employee.Post.Name}", LogLevel.Info);
            return CreatedAtAction("GetEmployee", new { id = employee.EmployeeId }, employee);
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.Include(p => p.Post).FirstOrDefaultAsync(p => p.EmployeeId == id);
            if (employee == null)
            {
                return BadRequest("Пользователь не найден");
            }

            if (TechnoWorldHub.ConnectedUsers.Any(p => p.Key == employee.Login))
            {

                LogService.LodMessage($"Провальная попытка удаления пользователя {employee.Login}: пользователь сейчас онлайн в системе.", LogLevel.Error);

                return BadRequest("Данный пользователь в данный момент онлайн в системе, удаление невозможно.");
            }

            _context.Employees.Remove(employee);
            LogService.LodMessage($"Было произведено удаление пользователя {employee.Login} с ролью {employee.Post.Name}", LogLevel.Info);
            await _hubContext.Clients.Group(SignalRGroups.storage_group).SendAsync("UpdateEmployees", "d");
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
