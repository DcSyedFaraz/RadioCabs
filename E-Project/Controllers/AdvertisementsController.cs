using E_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace E_Project.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;

        public AdvertisementsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }
        [HttpGet("approved/{id}")]
        public async Task<IActionResult> Approve(int id)
        {
           await UpdateStatus(id, "approved");
            return Ok(new { Message = "AD Status approved successfully!" });
        }
        [HttpGet("decline/{id}")]
        public async Task<IActionResult> Decline(int id)
        {
           await UpdateStatus(id, "declined");

            return Ok(new { Message = "AD Status declined successfully!" });
        }

        private async Task UpdateStatus(int id, string newStatus)
        {
            var status = _context.Advertisements.Find(id);

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
                        await userManager.AddToRoleAsync(newUser, UserRoles.User);

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
            var companies = _context.Advertisements.Where(c => c.Status != "approved").OrderByDescending(c => c.Id).ToList();
            return Ok(companies);
        }

        // GET: api/Advertisements
        [HttpGet("front")]
        public async Task<ActionResult<IEnumerable<Advertisement>>> GetAdvertisement()
        {
            var advertisement = await _context.Advertisements.Include(a => a.User).Where(c => c.Status == "approved").OrderByDescending(c => c.Id).ToListAsync();
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
            };

            var jsonString = JsonSerializer.Serialize(advertisement, options);


            return Ok(jsonString);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Advertisement>>> GetADBack()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "admin")
            {
                var AD = await _context.Advertisements.Include(a => a.User).Where(c => c.Status == "approved").OrderByDescending(c => c.Id).ToListAsync();
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                };

                var jsonString = JsonSerializer.Serialize(AD, options);

                return Ok(jsonString);
            }
            else
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var AD = await _context.Advertisements.Where(d => d.UserId == userId).Include(a => a.User).ToListAsync();
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                };

                var jsonString = JsonSerializer.Serialize(AD, options);

                return Ok(jsonString);
            }
        }

        // GET: api/Advertisements/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Advertisement>> GetAdvertisement(int id)
        {
            var advertisement = await _context.Advertisements.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == id);

            if (advertisement == null)
            {
                return NotFound();
            }
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
            };

            var jsonString = JsonSerializer.Serialize(advertisement, options);

            return Ok(jsonString);
        }

        // PUT: api/Advertisements/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdvertisement(int id, Advertisement advertisement)
        {
            if (id != advertisement.Id)
            {
                return BadRequest();
            }

            _context.Entry(advertisement).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdvertisementExists(id))
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
                Message = "Successfully Updated Advertisement's Data"
            });
        }

        // POST: api/Advertisements
        //[Authorize]
        [HttpPost]
        public async Task<ActionResult<Advertisement>> PostAdvertisement(Advertisement? advertisement)
        {


            var existingAD = await _context.Advertisements.FirstOrDefaultAsync(c => c.Email == advertisement.Email);
            if (existingAD != null)
            {
                return BadRequest(new { Message = "Advertiser with the same Email already exists." });
            }
            _context.Advertisements.Add(advertisement);
            await _context.SaveChangesAsync();

            return Ok(new Responce
            {
                Status = "Success",
                Message = "Successfully Created Advertisement"
            });
        }


        // DELETE: api/Advertisements/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdvertisement(int id)
        {
            var advertisement = await _context.Advertisements.FindAsync(id);
            if (advertisement == null)
            {
                return NotFound();
            }
            var userssid = advertisement.UserId;

            _context.Advertisements.Remove(advertisement);
            await _context.SaveChangesAsync();

            if (userssid != null)
            {
                var user = await userManager.FindByIdAsync(userssid);
                await userManager.DeleteAsync(user);
            }

            return Ok(new Responce
            {
                Status = "Success",
                Message = "Successfully Deleted Advertisement's Data"
            });
        }

        private bool AdvertisementExists(int id)
        {
            return _context.Advertisements.Any(e => e.Id == id);
        }
    }
}
