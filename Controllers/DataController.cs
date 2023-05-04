using Lab3.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lab3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;

        public DataController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetUserInfo()
        {
            var user = await userManager.GetUserAsync(User);

            return Ok(new string[] {
               user!.UserName,
               user!.Address,
               user!.Email
        });
        }
        [HttpGet("ForAdmin")]
        [Authorize(Policy = "Admin")]

        public async Task<ActionResult> GetInfoForManager()
        {
            var user = await userManager.GetUserAsync(User);

            return Ok(new string[] {
                 "This Data From Managers Only"
        });
        }

        [HttpGet("ForUser")]
        [Authorize(Policy = "User")]
        public async Task<ActionResult> GetInfoForUser()
        {
            var user = await userManager.GetUserAsync(User);

            return Ok(new string[] {
                 "This Data From Managers And Users"
        });
        }
    }
}
