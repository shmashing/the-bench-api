using TheBench.Logic.Adapters;
using TheBench.Logic.Models;

namespace TheBench.Logic.Services;

public class UserService(IUserAdapter userAdapter)
{
    public void InitializeDatabase()
    {
        userAdapter.SeedDatabase();
    }
    
    public async Task<User?> GetUserById(string userId)
    {
        var user = await userAdapter.GetUser(userId);
        return user;
    }

    public async Task<User> CreateUser(User user)
    {
        var newUser = await userAdapter.CreateUser(user);
        return newUser;
    }
}