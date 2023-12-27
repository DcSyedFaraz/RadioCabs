using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Project.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.AspNetCore.Identity;

namespace E_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriversController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;

        public DriversController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }
        [HttpGet("approved/{id}")]
        public async Task<IActionResult> Approve(int id)
        {
           await UpdateStatus(id, "approved");
            return Ok(new { Message = "Driver Status approved successfully!" });
        }
        [HttpGet("decline/{id}")]
        public async Task<IActionResult> Decline(int id)
        {
           await UpdateStatus(id, "declined");

            return Ok(new { Message = "Driver Status declined successfully!" });
        }

        private async Task UpdateStatus(int id, string newStatus)
        {
            var status = _context.Drivers.Find(id);

            if (status != null)
            {
                if (newStatus == "approved")
                {
                    var newUser = new ApplicationUser
                    {
                        UserName = status.Email,
                        Email = status.Email,
                        SecurityStamp = Guid.NewGuid().ToString(),
                    };

                    var result = await userManager.CreateAsync(newUser, "12345678");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newUser, UserRoles.Driver);

                        status.UserId = newUser.Id;
                    }
                    else
                    {
                        // Handle the case where user creation failed
                        // Log errors or take appropriate action

                        foreach (var error in result.Errors)
                        {
                            // Log each error or handle it as needed
                            Console.WriteLine($"Error: {error.Description}");
                        }

                        // Optionally, you can throw an exception to propagate the error
                         throw new ApplicationException("User creation failed");
                    }
                }

                status.Status = newStatus;

                _context.SaveChanges();
            }
        }

        [HttpGet("request")]
        public IActionResult request()
        {
            var companies = _context.Drivers.Where(c => c.Status != "approved").OrderByDescending(c => c.Id).ToList();
            return Ok(companies);
        }

        // GET: api/Drivers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Driver>>> GetDriverBack()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "admin") 
            {
                var driver = await _context.Drivers.Include(a => a.User).Where(c => c.Status == "approved").OrderByDescending(c => c.Id).ToListAsync();
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                };

                var jsonString = JsonSerializer.Serialize(driver, options);

                return Ok(jsonString);
            }
            else
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var driver = await _context.Drivers.Where(d => d.UserId == userId).Include(a => a.User).ToListAsync();
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                };

                var jsonString = JsonSerializer.Serialize(driver, options);

                return Ok(jsonString);
            }
        }


        [HttpGet("front")]
        public async Task<ActionResult<IEnumerable<Driver>>> GetDriver()
        {

            var driver = await _context.Drivers.Include(a => a.User).Where(c => c.Status == "approved").OrderByDescending(c => c.Id).ToListAsync();
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
            };

            var jsonString = JsonSerializer.Serialize(driver, options);

            return Ok(jsonString);

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
                Message = "Successfully Updated Driver's Data"
            });
        }

        // POST: api/Drivers
        // [Authorize]
        [HttpPost]
        public async Task<ActionResult<Driver>> PostDriver(Driver driver)
        {
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ////return Ok(userId);
            //// Set the UserId in the Advertisement object
            //driver.UserId = userId;

            var existingdriver = await _context.Drivers.FirstOrDefaultAsync(c => c.Email == driver.Email);
            if (existingdriver != null)
            {
                return BadRequest(new { Message = "Driver with the same Email already exists." });
            }
            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();

            // return CreatedAtAction("GetDriver", new { id = driver.Id }, driver);
            return Ok(new Responce
            {
                Status = "Success",
                Message = "Successfully Created Driver's Data"
            });
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
            var userssid = driver.UserId;

            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();
            if (userssid != null)
            {
                var user = await userManager.FindByIdAsync(userssid);
                await userManager.DeleteAsync(user);
            }

            return Ok(new Responce
            {
                Status = "Success",
                Message = "Successfully Deleted Driver's Data"
            });
        }

        private bool DriverExists(int id)
        {
            return _context.Drivers.Any(e => e.Id == id);
        }
    }
}
