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

    public List<UserProfile> FindUsers(UserQuery query)
    {
        var dayTime = new DailyAvailability(query.Day, query.TimeWindow, Availability.Available);

        var usersQuery = userContext.UserProfiles
            .AsEnumerable()
            .ToArray();
        
        var resultSet = usersQuery
            .Where(u => u.Sports.Contains(query.Sport))
            .Where(u => u.Schedule.DailyAvailability.Contains(dayTime));
        
        return resultSet.ToList();
    }
    
    public async Task<UserProfile?> GetUserProfile(string userId)
    {
        return await userContext.UserProfiles
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }
}