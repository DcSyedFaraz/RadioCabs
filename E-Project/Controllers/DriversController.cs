using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Project.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using E_Project.Migrations;

namespace E_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriversController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DriversController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Drivers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Driver>>> GetDriver()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
           // return Ok(some);
            if (role == "admin") // Use double equals for comparison
            {
                var driver = await _context.Drivers.Include(a => a.User).ToListAsync();
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                };

                var jsonString = JsonSerializer.Serialize(driver, options);

                return Ok(jsonString);
            }
            else
            {
                return Ok("done"); // Use double quotes for string literals
            }
        }


        // GET: api/Drivers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Driver>> GetDriver(int id)
        {
            var driver = await _context.Drivers.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == id);

            if (driver == null)
            {
                return NotFound();
            }

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
            };

            var jsonString = JsonSerializer.Serialize(driver, options);

            return Ok(jsonString);
        }

        // PUT: api/Drivers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDriver(int id, Driver driver)
        {
            if (id != driver.Id)
            {
                return BadRequest(driver);
            }

            _context.Entry(driver).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DriverExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new Responce
            {
                Status = "Success",
                Message = "SUccessfully Updated Driver's Data"
            }); ;
        }

        // POST: api/Drivers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Driver>> PostDriver(Driver driver)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //return Ok(userId);
            // Set the UserId in the Advertisement object
            driver.UserId = userId;
            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDriver", new { id = driver.Id }, driver);
        }

        // DELETE: api/Drivers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null)
            {
                return NotFound();
            }

            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();

            return Ok(new Responce
            {
                Status = "Success",
                Message = "SUccessfully Deleted Driver's Data"
            });
        }

        private bool DriverExists(int id)
        {
            return _context.Drivers.Any(e => e.Id == id);
        }
    }
}
