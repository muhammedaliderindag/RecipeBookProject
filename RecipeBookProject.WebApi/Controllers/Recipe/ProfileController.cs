using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeBookProject.Business.Abstract;
using RecipeBookProject.Business.Models;

namespace RecipeBookProject.WebApi.Controllers.Recipe
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet("getUserProfile")]
        public async Task<IActionResult> GetUserProfile([FromQuery] int? userid)
        {
            var response = await _profileService.GetUserProfileAsync(userid);
            return Ok(response);
        }
        [HttpGet("GetUserDetails")]
        public async Task<IActionResult> GetUserDetails([FromQuery] int? userid)
        {
            var response = await _profileService.GetUserDetailAsync(userid);
            return Ok(response);
        }
        [HttpPost("UpdateProfileDetails")]
        public async Task<IActionResult> UpdateProfileDetails([FromBody] UserDetails a,[FromQuery] int? userid)
        {
            var response = await _profileService.UpdateProfileDetailsAsync(a, userid);
            return Ok(response);
        }
    }
}
