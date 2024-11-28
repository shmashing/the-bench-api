using TheBench.Logic.Database;
using TheBench.Logic.Models;

namespace TheBench.Logic.Adapters;

public class UserAdapter(UserContext userContext) : IUserAdapter
{
    public void SeedDatabase()
    {
        if (userContext.Users.Any()) return;
        
        var users = new User[]
        {
            new("Alice", "Smith", "1", "alice1@gmail.com", "123", "location1", GenerateSchedule(), new List<Sport> { Sport.Softball, Sport.Pickleball }),
            new("Bob", "Johnson", "2", "bob2@gmail.com", "123", "location2", GenerateSchedule(), new List<Sport> { Sport.Kickball }),
            new("Charlie", "Williams", "3", "charlie3@gmail.com", "123", "location3", GenerateSchedule(), new List<Sport> { Sport.Softball }),
            new("David", "Brown", "4", "david4@gmail.com", "123", "location4", GenerateSchedule(), new List<Sport> { Sport.Pickleball }),
            new("Eve", "Jones", "5", "eve5@gmail.com", "123", "location5", GenerateSchedule(), new List<Sport> { Sport.Kickball, Sport.Softball }),
            new("Frank", "Garcia", "6", "frank6@gmail.com", "123", "location6", GenerateSchedule(), new List<Sport> { Sport.Softball, Sport.Pickleball }),
            new("Grace", "Martinez", "7", "grace7@gmail.com", "123", "location7", GenerateSchedule(), new List<Sport> { Sport.Kickball }),
            new("Hank", "Rodriguez", "8", "hank8@gmail.com", "123", "location8", GenerateSchedule(), new List<Sport> { Sport.Softball }),
            new("Ivy", "Wilson", "9", "ivy9@gmail.com", "123", "location9", GenerateSchedule(), new List<Sport> { Sport.Pickleball }),
            new("Jack", "Lopez", "10", "jack10@gmail.com", "123", "location10", GenerateSchedule(), new List<Sport> { Sport.Kickball, Sport.Softball }),
            new("Kathy", "Gonzalez", "11", "kathy11@gmail.com", "123", "location11", GenerateSchedule(), new List<Sport> { Sport.Softball, Sport.Pickleball }),
            new("Leo", "Perez", "12", "leo12@gmail.com", "123", "location12", GenerateSchedule(), new List<Sport> { Sport.Kickball }),
            new("Mia", "Sanchez", "13", "mia13@gmail.com", "123", "location13", GenerateSchedule(), new List<Sport> { Sport.Softball }),
            new("Nina", "Clark", "14", "nina14@gmail.com", "123", "location14", GenerateSchedule(), new List<Sport> { Sport.Pickleball }),
            new("Oscar", "Ramirez", "15", "oscar15@gmail.com", "123", "location15", GenerateSchedule(), new List<Sport> { Sport.Kickball, Sport.Softball }),
            new("Paul", "Lewis", "16", "paul16@gmail.com", "123", "location16", GenerateSchedule(), new List<Sport> { Sport.Softball, Sport.Pickleball }),
            new("Quinn", "Walker", "17", "quinn17@gmail.com", "123", "location17", GenerateSchedule(), new List<Sport> { Sport.Kickball }),
            new("Rose", "Hall", "18", "rose18@gmail.com", "123", "location18", GenerateSchedule(), new List<Sport> { Sport.Softball }),
            new("Sam", "Allen", "19", "sam19@gmail.com", "123", "location19", GenerateSchedule(), new List<Sport> { Sport.Pickleball }),
            new("Tina", "Young", "20", "tina20@gmail.com", "123", "location20", GenerateSchedule(), new List<Sport> { Sport.Kickball, Sport.Softball })
        };

        userContext.Users.AddRange(users);
        userContext.SaveChanges();
    }

    public async Task<User?> GetUser(string id)
    {
        var user = await userContext.Users.FindAsync(id);
        
        return user;
    }

    public async Task<User> CreateUser(User user)
    {
        await userContext.Users.AddAsync(user);
        await userContext.SaveChangesAsync();
        
        return (await userContext.Users.FindAsync(user.Id))!;
    }

    private static Schedule GenerateSchedule()
    {
        var random = new Random();
        var dailyAvailability = new List<DailyAvailability>();
    
        foreach (var day in Enum.GetValues<Day>())
        {
            foreach (var timeWindow in Enum.GetValues<TimeWindow>())
            {
                var availability = (Availability)random.Next(Enum.GetValues<Availability>().Length);
                dailyAvailability.Add(new DailyAvailability(day, timeWindow, availability));
            }
        }
    
        return new Schedule(dailyAvailability);
    }
}