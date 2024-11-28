namespace TheBench.Logic.Models;

public record Schedule(
    List<DailyAvailability> DailyAvailability
);

public record DailyAvailability(
    Day Day,
    TimeWindow TimeWindow,
    Availability Availability
);

public enum Day
{
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday,
    Sunday
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