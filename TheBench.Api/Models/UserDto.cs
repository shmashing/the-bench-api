using Microsoft.OpenApi.Extensions;
using TheBench.Logic.Models;

namespace TheBench.Api.Models;

public class UserDto(User user)
{
    public string Id => user.Id;
    public string Name => user.FirstName + " " + user.LastName;
    public List<string> Sports => user.Sports.ConvertAll(s => s.GetDisplayName());
    public string Location => user.Location;
    public List<string> AvailableTimes => SetAvailableTimes(Availability.Available);
    public List<string> UnavailableTimes => SetAvailableTimes(Availability.Unavailable);
    public List<string> PartiallyAvailableTimes => SetAvailableTimes(Availability.Partially);


    private List<string> SetAvailableTimes(Availability availability)
    {
        var availableTimes = new List<string>();
        if (user.Schedule == null) return availableTimes;
        
        var matchedAvailability = user.Schedule.DailyAvailability.FindAll(a => a.Availability == availability);
        availableTimes.AddRange(matchedAvailability.ConvertAll(a => $"{a.Day.GetDisplayName()} {a.TimeWindow.GetDisplayName()}"));

        return availableTimes;
    }
}