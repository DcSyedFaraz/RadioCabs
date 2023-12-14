﻿using E_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace E_Project.Controllers
{
    //[Authorize]
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

        [HttpGet("companies")]
        public IActionResult GetCompanies()
        {
            var companies = _context.Companies.Include(c => c.User).ToList();
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
            };

            var jsonString = JsonSerializer.Serialize(companies, options);


            return Ok(jsonString);
        }

        [HttpPost("register-company")]
        public async Task<IActionResult> RegisterCompany([FromBody] Company company)
        {
            if (ModelState.IsValid)
            {
                var existingCompany = await _context.Companies.FirstOrDefaultAsync(c => c.Name == company.Name);
                if (existingCompany != null)
                {
                    return BadRequest(new { Message = "Company with the same name already exists." });
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
                    await userManager.AddToRoleAsync(newUser, UserRoles.User);

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




    }
}
