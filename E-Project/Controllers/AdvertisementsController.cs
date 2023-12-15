using E_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace E_Project.Controllers
{
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdvertisementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Advertisements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Advertisement>>> GetAdvertisement()
        {
            return await _context.Advertisements.ToListAsync();
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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

            return NoContent();
        }

        // POST: api/Advertisements
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Advertisement>> PostAdvertisement(Advertisement advertisement)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //return Ok(userId);
            // Set the UserId in the Advertisement object
            advertisement.UserId = userId;
            _context.Advertisements.Add(advertisement);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAdvertisement", new { id = advertisement.Id }, advertisement);
        }

        private ActionResult<Advertisement> OK(string? userId)
        {
            throw new NotImplementedException();
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

            _context.Advertisements.Remove(advertisement);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdvertisementExists(int id)
        {
            return _context.Advertisements.Any(e => e.Id == id);
        }
    }
}
