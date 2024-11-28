using TheBench.Logic.Models;

namespace TheBench.Logic.Adapters;

public interface IUserAdapter
{
    public void SeedDatabase();
    public Task<User?> GetUser(string id);
    public Task<User> CreateUser(User user);
}