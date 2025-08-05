using TheBench.Logic.Models;

namespace TheBench.Logic.Adapters;

public interface IUserAdapter
{
    public Task<UserProfile> CreateUserProfile(UserProfile userProfile);
    public Task<List<UserProfile>> GetUsers(List<string> userIds);
    public Task<UserProfile?> GetUserProfile(string userId);
    public Task<UserProfile?> GetUserProfileByAuthId(string authId);
    public Task<List<UserProfile>> FindUsers(string searchTerm);
}