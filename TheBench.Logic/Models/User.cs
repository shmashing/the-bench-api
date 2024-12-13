namespace TheBench.Logic.Models;

public record User(
    string FirstName, 
    string LastName, 
    string Id,
    string Email,
    string PhoneNumber,
    string Location,
    
    Schedule Schedule,
    List<Sport> Sports
    );