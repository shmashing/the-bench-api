using TheBench.Logic.Adapters;
using TheBench.Logic.Models;

namespace TheBench.Logic.Services;

public class SearchService(IUserAdapter userAdapter)
{
    public List<User> FindUsers(string sport, string day, string time)
    {
        Console.WriteLine($"Searching for {sport} players for {day} at {time}");
        if (string.IsNullOrWhiteSpace(sport) || !Enum.TryParse(sport, out Sport sportEnum))
        {
            Console.WriteLine($"Sport failed validation:");
            return [];
        }

        if (string.IsNullOrWhiteSpace(day) || !Enum.TryParse(day, out Day dayEnum))
        {
            Console.WriteLine("Day failed validation");
            return [];
        }

        if (!TryParseTimeWindow(time, out var timeWindow))
        {
            Console.WriteLine("Time failed validation");
            return [];
        }

        var query = new UserQuery(sportEnum, dayEnum, timeWindow);
        Console.WriteLine($"Translating search query to {sportEnum} players for {dayEnum} {timeWindow}");

        return userAdapter.FindUsers(query);
    }
    
    private static bool TryParseTimeWindow(string timeWindow, out TimeWindow timeWindowEnum)
    {
        timeWindowEnum = TimeWindow.Morning;
        
        if (Enum.TryParse(timeWindow, out TimeWindow parsedEnum))
        {
            timeWindowEnum = parsedEnum;
            return true;
        }
        
        if (string.IsNullOrWhiteSpace(timeWindow) || !int.TryParse(timeWindow, out var timeInt)) return false;
        
        switch (timeInt)
        {
            case >= 2400:
                return false;
            case < 1200:
                timeWindowEnum = TimeWindow.Morning;
                return true;
            case < 1700:
                timeWindowEnum = TimeWindow.Afternoon;
                return true;
            default:
                timeWindowEnum = TimeWindow.Evening;
                return true;
        }
    }
}