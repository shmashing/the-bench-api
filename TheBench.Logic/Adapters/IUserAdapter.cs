using TheBench.Logic.Models;

namespace TheBench.Logic.Adapters;

public interface IUserAdapter
{
    public Task<UserProfile> CreateUserProfile(UserProfile userProfile);
    public List<UserProfile> FindUsers(UserQuery query);
    public Task<UserProfile?> GetUserProfile(string userId);
}