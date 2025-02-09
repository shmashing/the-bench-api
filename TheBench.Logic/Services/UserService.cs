using System.Collections.Specialized;
using System.Text.Json;
using TheBench.Logic.Adapters;
using TheBench.Logic.Models;
using TheBench.Logic.Requests.V1;

namespace TheBench.Logic.Services;

public class UserService(IUserAdapter userAdapter, IdService idService)
{
    public async Task<UserProfile> CreateUserProfile(CreateUserProfileRequest request)
    {
        var id = idService.Generate("user");
        var schedule = request.Schedule ?? Schedule.FullAvailability();
        var userProfile = new UserProfile(id, request.UserId, request.Location, request.Gender, request.Sports, schedule);
       
        return await userAdapter.CreateUserProfile(userProfile);
    }

    public async Task<UserProfile> GetUserProfile(string userId)
    {
        return await userAdapter.GetUserProfile(userId);
    }
}