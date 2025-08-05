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
        var userProfile = new UserProfile(
            id, 
            request.Auth0Id, 
            request.FirstName,
            request.LastName,
            request.Email,
            request.Gender,
            request.Avatar
            );
       
        var createdProfile = await userAdapter.CreateUserProfile(userProfile);
        return GetUserProfileResponse(createdProfile);
    }

    public async Task<UserProfileResponse?> GetUserProfile(string userId)
    {
        var userProfile = await userAdapter.GetUserProfile(userId);
        return userProfile == null ? null : GetUserProfileResponse(userProfile);
    }
    
    public async Task<UserProfileResponse?> GetUserProfileByAuthId(string authId)
    {
        var userProfile = await userAdapter.GetUserProfileByAuthId(authId);
        return userProfile == null ? null : GetUserProfileResponse(userProfile);
    }
    
    public async Task<List<UserProfileResponse>> FindUsers(string searchTerm)
    {
        var users = await userAdapter.FindUsers(searchTerm);
        return users.Select(GetUserProfileResponse).ToList();
    }

    private static UserProfileResponse GetUserProfileResponse(UserProfile userProfile)
    {
        return new UserProfileResponse(
            userProfile.Id,
            userProfile.Auth0Id,
            userProfile.FirstName,
            userProfile.LastName,
            userProfile.Email,
            userProfile.Gender
        );
    }
}