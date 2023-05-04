using Lab3.Data.Models;
using Lab3.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Lab3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;

        public UserController(UserManager<ApplicationUser> userManager , IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }
        [HttpPost("Admin")]
        public async Task<ActionResult> AdminRegister(RegisterDto registerDto)
        {
            var newEmployee = new ApplicationUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                Address = registerDto.Address,
            };

            var creationResult = await userManager.CreateAsync(newEmployee,
                registerDto.Password);
            if (!creationResult.Succeeded)
            {
                return BadRequest(creationResult.Errors);
            }

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, newEmployee.Id),
            new Claim(ClaimTypes.Role, "Admin"),
        };

            await userManager.AddClaimsAsync(newEmployee, claims);

            return NoContent();
        }

        [HttpPost("User")]
        public async Task<ActionResult> UserRegister(RegisterDto registerDto)
        {
            var newEmployee = new ApplicationUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                Address = registerDto.Address,
            };

            var creationResult = await userManager.CreateAsync(newEmployee,
                registerDto.Password);
            if (!creationResult.Succeeded)
            {
                return BadRequest(creationResult.Errors);
            }

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, newEmployee.Id),
            new Claim(ClaimTypes.Role, "User"),
        };

            await userManager.AddClaimsAsync(newEmployee, claims);

            return NoContent();
        }

        [HttpPost("Login")]
        public async Task<ActionResult<TokenDto>> Login(LoginDto credentials)
        {
            ApplicationUser? user = await userManager.FindByNameAsync(credentials.UserName);

            if (user == null)
            {
                return BadRequest();
            }

            bool isPasswordCorrect = await userManager.CheckPasswordAsync(user, credentials.Password);
            if (!isPasswordCorrect)
            {
                return BadRequest();
            }

            var claimsList = await userManager.GetClaimsAsync(user);
            return GenerateToken(claimsList);
        }

        private TokenDto GenerateToken(IList<Claim> claimsList)
        {
            string keyString = configuration.GetValue<string>("SecretKey") ?? string.Empty;
            var keyInBytes = Encoding.ASCII.GetBytes(keyString);
            var key = new SymmetricSecurityKey(keyInBytes);

            var signingCredentials = new SigningCredentials(key,
                SecurityAlgorithms.HmacSha256Signature);

            var expiry = DateTime.Now.AddMinutes(30);

            var jwt = new JwtSecurityToken(
                    expires: expiry,
                    claims: claimsList,
                    signingCredentials: signingCredentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(jwt);

            return new TokenDto
            {
                Token = tokenString,
                Expiry = expiry
            };
        }



    }
}
