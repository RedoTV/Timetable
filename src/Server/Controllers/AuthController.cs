using Microsoft.AspNetCore.Mvc;
using TimetableServer.Models.Responses;
using TimetableServer.Models.Requests;
using TimetableServer.Services.Interfaces;

namespace TimetableServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    IIdentityService IdentityService;
    public AuthController(IIdentityService identityService)
    {
        IdentityService = identityService;
    }

    [HttpPost]
    [Route("signin")]
    public async Task<IActionResult> SignIn(UserRequestForm user)
    {
        LoginResponse response = await IdentityService.LoginUser(user);
        if(response is not null)
            return Ok(response);
        else return NotFound("User not found");
    }

    [HttpPost]
    [Route("register")]
    public async Task<LoginResponse> Register(UserRequestForm user)
    {
        return await IdentityService.RegisterUser(user);
    }
}
