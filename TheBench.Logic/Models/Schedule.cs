namespace TheBench.Logic.Models;

public class Schedule(List<DailyAvailability> dailyAvailability)
{
    public List<DailyAvailability> DailyAvailability { get; set; } = dailyAvailability;

    public static Schedule FullAvailability()
    {
        var dailyAvailability = new List<DailyAvailability>();
        foreach (var day in Enum.GetValues<Day>())
        {
            foreach (var timeWindow in Enum.GetValues<TimeWindow>())
            {
                dailyAvailability.Add(new DailyAvailability(day, timeWindow, Availability.Available));
            }
        }
        return new Schedule(dailyAvailability);
    }
}

public record DailyAvailability(
    Day Day,
    TimeWindow TimeWindow,
    Availability Availability
);

public enum Day
{
    Monday = 0,
    Tuesday = 1,
    Wednesday = 2,
    Thursday = 3,
    Friday = 4,
    Saturday = 5,
    Sunday = 6
}

public enum TimeWindow
{
    Morning,
    Afternoon,
    Evening
}

public enum Availability
{
    Available,
    Unavailable,
    Partially
}