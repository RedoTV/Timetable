using IdentityApi.Models.DbModels;
using IdentityApi.Models.Requests;
using IdentityApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApi.Controllers;

[ApiController]
[Route("[controller]")]
public class IdentityController : ControllerBase
{
    private readonly ILogger<IdentityController> _logger;
    private readonly IIdentityService _identityService;
    public IdentityController(ILogger<IdentityController> logger, IConfiguration configuration, IIdentityService identityService)
    {
        _logger = logger;
        _identityService = identityService;
    }

    [HttpPost("SignIn")]
    public async Task<IActionResult> SignIn(UserFormInfo userForm)
    {
        User? user = await _identityService.GetUserAsync(userForm);
        _logger.LogInformation("signing user: ", user);
        if(user == null)
            return NotFound("User not found");

        string jwt = await _identityService.GetTokenAsync(user);
        return Ok(jwt);
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(UserFormInfo userForm)
    {
        _logger.LogInformation("Start registering user");
        string? jwt = await _identityService.RegisterUserAsync(userForm);
        if(jwt == null)
            return BadRequest();

        _logger.LogInformation("User registered");
        return Ok(jwt);
    }
}
