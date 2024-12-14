using System.Text.Json;
using TheBench.Logic.Adapters;
using TheBench.Logic.Models;

namespace TheBench.Logic.Services;

public class UserService(IUserAdapter userAdapter, IdService idService)
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

    public async Task<User> CreateUser(AuthenticatedUser authenticatedUser)
    {
        var user = MapAuthenticatedUser(authenticatedUser);
        var newUser = await userAdapter.CreateUser(user);
        return newUser;
    }

    private User MapAuthenticatedUser(AuthenticatedUser authenticatedUser)
    {
        var nameParts = authenticatedUser.Name.Split(" ");
        var firstName = nameParts[0];
        var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;
        var newId = idService.Generate("user");
        var schedule = Schedule.FullAvailability();
        
        return new User(
            firstName,
            lastName,
            newId,
            authenticatedUser.Email,
            authenticatedUser.PhoneNumber,
            authenticatedUser.City,
            schedule,
            authenticatedUser.Sports
        );
    }
}