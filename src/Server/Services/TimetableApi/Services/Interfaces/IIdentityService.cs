using TimetableServer.Models.Requests;
using TimetableServer.Models.Responses;

namespace TimetableServer.Services.Interfaces;

public interface IIdentityService
{
    public Task<LoginResponse> LoginUser(UserRequestForm userRequest);
    public Task<LoginResponse> RegisterUser(UserRequestForm userRequest);
}
