using System.Collections.Specialized;
using System.Text.Json;
using TheBench.Logic.Adapters;
using TheBench.Logic.Models;
using TheBench.Logic.Requests.V1;
using TheBench.Logic.Responses;

namespace TheBench.Logic.Services;

public class UserService(IUserAdapter userAdapter, IdService idService)
{
    public async Task<UserProfileResponse> CreateUserProfile(CreateUserProfileRequest request)
    {
        var id = idService.Generate("user");
        var schedule = request.Schedule ?? Schedule.FullAvailability();
        var userProfile = new UserProfile(id, request.UserId, request.Location, request.Gender, request.Sports, schedule);
       
        var createdProfile = await userAdapter.CreateUserProfile(userProfile);
        return GetUserProfileResponse(createdProfile);
    }

    public async Task<UserProfileResponse?> GetUserProfile(string userId)
    {
        var userProfile = await userAdapter.GetUserProfile(userId);
        return userProfile == null ? null : GetUserProfileResponse(userProfile);
    }

    private static UserProfileResponse GetUserProfileResponse(UserProfile userProfile)
    {
        var schedule = userProfile.Schedule.DailyAvailability
            .ToDictionary(
                key => $"{key.Day}|{key.TimeWindow}",
                value => value.Availability
            );
        return new UserProfileResponse(
            userProfile.Id,
            userProfile.UserId,
            userProfile.Location,
            userProfile.Gender,
            userProfile.Sports,
            schedule
        );
    }
}