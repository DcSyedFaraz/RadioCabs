using E_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_Project.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticateController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;

        public AuthenticateController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExist = await userManager.FindByNameAsync(model.UserName);
            if (userExist != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Responce { Status = "Error", Message = "User Already Exist!" });

            }
            ApplicationUser user = new ApplicationUser()
            {
                Email = model.EmailAddress,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName,
                //PasswordHash = model.Password
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    // Log the details of the error
                    Console.WriteLine($"Error: {error.Code}, Description: {error.Description}");
                }
                var errorsAsString = string.Join("; ", result.Errors.Select(error => $"{error.Code}: {error.Description}"));

                return StatusCode(StatusCodes.Status500InternalServerError,
                    value: new Responce { Status = "Error", Message = errorsAsString });
            }

            if (await roleManager.RoleExistsAsync(UserRoles.User))
            {
                await userManager.AddToRoleAsync(user, UserRoles.User);
            }

            return Ok(new Responce { Status = "200", Message = "User Created Successfully!" });
        }

        //[HttpPost]
        //[Route("register-admin")]

        //public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        //{
        //    var userExist = await userManager.FindByNameAsync(model.UserName);
        //    if (userExist != null)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, new Responce { Status = "Error", Message = "User Already Exist!" });

        //    }
        //    ApplicationUser user = new ApplicationUser()
        //    {
        //        Email = model.EmailAddress,
        //        SecurityStamp = Guid.NewGuid().ToString(),
        //        UserName = model.UserName,
        //        PasswordHash = model.Password,
        //        Designation = model.Designation
        //    };

        //    var result = await userManager.CreateAsync(user);

        //    if (!result.Succeeded)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //            new Responce { Status = "Error", Message = "User Creation Failed!" });




        //    }
        //    if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
        //        await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

        //    if (!await roleManager.RoleExistsAsync(UserRoles.User))
        //        await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

        //    if (await roleManager.RoleExistsAsync(UserRoles.Admin))
        //    {
        //        await userManager.AddToRoleAsync(user, UserRoles.Admin);
        //    }



        //    return Ok(new Responce { Status = "200", Message = "User Created Successfully!" });
        //}

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);
            var checkpass = await userManager.CheckPasswordAsync(user, model.Password);

            //if (user != null && model.Password == user.PasswordHash)
            if (user != null && checkpass)
            {
                var userRoles = await userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                     new Claim(ClaimTypes.Name, user.UserName),
                     new Claim(ClaimTypes.NameIdentifier, user.Id),

                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

                };

                authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: configuration["JWT:ValidIssuer"],
                    audience: configuration["JWT:ValidAudience"],
                    expires: DateTime.UtcNow.AddHours(24), // Adjust token expiration as needed
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo,
                    UserRoles = userRoles
                });
            }

            // Delay the response to mitigate timing attacks
            await Task.Delay(2000);

            return Unauthorized(new Responce
            {
                Status = "Error",
                Message = "Invalid username or password"
            });

        }

        [HttpDelete("delete-user/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                // Find the user by its Id
                var user = await userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new { Message = "User not found." });
                }

                // Remove the user from the Identity system
                var result = await userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return Ok(new { Message = "User deleted successfully!" });
                }
                else
                {
                    // If there are errors, log them or return an appropriate response
                    var errors = result.Errors.Select(error => $"{error.Code}: {error.Description}");
                    return StatusCode(500, new { Message = "Error deleting user.", Errors = errors });
                }
            }
            catch (Exception ex)
            {
                // Log the exception...
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }


    }

}
