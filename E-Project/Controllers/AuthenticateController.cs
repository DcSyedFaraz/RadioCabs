using E_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
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
                PasswordHash = model.Password
            };

            var result = await userManager.CreateAsync(user);

            if(!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Responce { Status = "Error", Message = "User Creation Failed!" });
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

            if (user != null && checkpass != true )
            {
                var userRole = await userManager.GetRolesAsync(user);

                var authclaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                foreach(var userR in userRole)
                {
                    authclaims.Add(new Claim(ClaimTypes.Role, userR));
                }
                var authSigninkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes
                    (configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: configuration["JWT:ValidIssuer"],
                    audience: configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddDays(1),
                    claims: authclaims,
                    signingCredentials: new SigningCredentials(authSigninkey, SecurityAlgorithms.HmacSha256
                    ));
             return Ok(
                new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    userRoles = userRole
                }
                );
            }
            return Unauthorized(
                new Responce
                {
                    Status = user.ToString(),
                    Message = checkpass.ToString()
                }
                );
        }
    }

}
