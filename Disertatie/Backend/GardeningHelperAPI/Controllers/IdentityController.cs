using DataExchange.DTOs.Request;
using DataExchange.DTOs.Response;
using GardeningHelperAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GardeningHelperAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IdentityService _identityService;

        public IdentityController(IdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequestDTO registerModel)
        {
            try
            {
                AuthResponseDTO response = await _identityService.Register(registerModel);
                return Ok(response);
            }
            catch
            {
                return BadRequest("Something went wrong");
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestDTO loginModel)
        {
            try
            {
                AuthResponseDTO response = await _identityService.Login(loginModel);
                return Ok(response);
            }
            catch
            {
                return BadRequest("Something went wrong");
            }
        }
    }
}
