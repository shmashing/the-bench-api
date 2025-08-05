using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using TheBench.Logic.Database;
using TheBench.Logic.Models;

namespace TheBench.Logic.Adapters;

public class UserAdapter(UserContext userContext) : IUserAdapter
{
    public async Task<UserProfile> CreateUserProfile(UserProfile userProfile)
    {
        userContext.UserProfiles.Add(userProfile);
        await userContext.SaveChangesAsync();
        return userProfile;
    }

    public async Task<List<UserProfile>> GetUsers(List<string> userIds)
    {
        var users = await userContext.UserProfiles.ToListAsync();
        return users.Where(u => userIds.Contains(u.Id)).ToList();
    }
    
    public async Task<UserProfile?> GetUserProfile(string userId)
    {
        return await userContext.UserProfiles
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
    
    public async Task<UserProfile?> GetUserProfileByAuthId(string authId)
    {
        return await userContext.UserProfiles
            .FirstOrDefaultAsync(u => u.Auth0Id == authId);
    }

    public async Task<List<UserProfile>> FindUsers(string searchTerm)
    {
        var users = await userContext.UserProfiles.ToListAsync();
        
        return users
            .Where(u => 
                u.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                u.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                Regex.IsMatch(u.Email, $"^{Regex.Escape(searchTerm)}", RegexOptions.IgnoreCase))
            .ToList();
    }
}