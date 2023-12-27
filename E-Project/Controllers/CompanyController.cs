using E_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace E_Project.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<Company> _logger;
        public CompanyController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<Company> logger)
        {
            _context = context;
            this.userManager = userManager;
            _logger = logger;

        }


        [HttpGet("search")]
        public IActionResult Search(string keyword)
        {
            var result = new
            {
                Drivers = _context.Drivers.Where(d => d.Name.Contains(keyword) && d.Status == "approved").ToList(),
                Advertisements = _context.Advertisements.Where(a => a.CompanyName.Contains(keyword) && a.Status == "approved").ToList(),
                Companies = _context.Companies.Where(c => c.Name.Contains(keyword) && c.Status == "approved").ToList()
            };

            return Ok(result);
        }




        [HttpGet("approved/{id}")]
        public IActionResult Approve(int id)
        {
            UpdateStatus(id, "approved");
            return Ok(new { Message = "Company Status approved successfully!" });
        }
        [HttpGet("decline/{id}")]
        public IActionResult Decline(int id)
        {
            UpdateStatus(id, "declined");

            return Ok(new { Message = "Company Status declined successfully!" });
        }

        private void UpdateStatus(int id, string newStatus)
        {
            var status = _context.Companies.Find(id);

            if (status != null)
            {
                status.Status = newStatus;
                _context.SaveChanges();
            }
        }
        [HttpGet("request")]
        public IActionResult request()
        {
            var companies = _context.Companies.Where(c => c.Status != "approved").OrderByDescending(c => c.Id).ToList();
            return Ok(companies);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanyBack()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "admin")
            {
                var company = await _context.Companies.Include(a => a.User).Where(c => c.Status == "approved").OrderByDescending(c => c.Id).ToListAsync();
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                };

                var jsonString = JsonSerializer.Serialize(company, options);

                return Ok(jsonString);
            }
            else
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var company = await _context.Companies.Where(d => d.UserId == userId).Include(a => a.User).ToListAsync();
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                };

                var jsonString = JsonSerializer.Serialize(company, options);

                return Ok(jsonString);
            }
        }

        [HttpGet("companies")]
        public IActionResult GetCompanies()
        {
            var companies = _context.Companies.Include(c => c.User).Where(c => c.Status == "approved").OrderByDescending(c => c.Id).ToList();
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
            };

            var jsonString = JsonSerializer.Serialize(companies, options);


            return Ok(jsonString);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            var companies = await _context.Companies.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == id);

            if (companies == null)
            {
                return NotFound();
            }
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
            };

            var jsonString = JsonSerializer.Serialize(companies, options);

            return Ok(jsonString);
        }

        [HttpPost("register-company")]
        public async Task<IActionResult> RegisterCompany([FromBody] Company? company)
        {
            if (ModelState.IsValid)
            {
                var existingCompany = await _context.Companies.FirstOrDefaultAsync(c => c.Email == company.Email);
                if (existingCompany != null)
                {
                    return BadRequest(new { Message = "Company with the same Email already exists." });
                }

                _context.Companies.Add(company);
                await _context.SaveChangesAsync();

                var newUser = new ApplicationUser
                {
                    UserName = company.Email,
                    Email = company.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };

                var result = await userManager.CreateAsync(newUser, "12345678");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, UserRoles.Company);


                    company.UserId = newUser.Id;
                    await _context.SaveChangesAsync();

                    return Ok(new { Message = "Company registered successfully!" });
                }
                else
                {
                    var errorsAsString = string.Join("; ", result.Errors.Select(error => $"{error.Code}: {error.Description}"));
                    // Log errors
                    _logger.LogError($"User creation failed. Errors: {errorsAsString}");

                    return BadRequest(new { Message = errorsAsString });
                }
            }

            return BadRequest(new { Message = "Invalid model state." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(int id, Company company)
        {
            if (id != company.Id)
            {
                return BadRequest();
            }

            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
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
                Message = "Successfully Updated Company's Data"
            });
        }

        [HttpDelete("delete-company/{companyId}")]
        public async Task<IActionResult> DeleteCompany(int companyId)
        {
            //try
            //{
            // Find the company by its Id
            var company = _context.Companies.Find(companyId);

            if (company == null)
            {
                return NotFound(new { Message = "Company not found." });
            }
            var userssid = company.UserId;

            // Remove the company from the database
            _context.Companies.Remove(company);
            _context.SaveChanges();
            if (userssid != null)
            {
                var user = await userManager.FindByIdAsync(userssid);
                await userManager.DeleteAsync(user);
            }

            return Ok(new { Message = "Company deleted successfully!" });
            //}
            //catch (Exception ex)
            //{
            //    // Log the exception...
            //    return StatusCode(500, new { Message = "Internal server error." });
            //}
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }


    }
}
