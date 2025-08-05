using Microsoft.EntityFrameworkCore;
using TheBench.Logic.Database;
using TheBench.Logic.Models;

namespace TheBench.Logic.Adapters;

public class SubstituteRequestAdapter(UserContext dbContext) : ISubstituteRequestAdapter
{
    public async Task<SubstituteRequest> CreateSubstituteRequest(SubstituteRequest substituteRequest)
    {
        dbContext.SubstituteRequests.Add(substituteRequest);
        await dbContext.SaveChangesAsync();
        
        return substituteRequest;
    }

    public async Task<List<SubstituteRequest>> GetOpenSubstituteRequests()
    {
        var requests = await dbContext.SubstituteRequests.ToListAsync();
        return requests;
    }

    public async Task<SubstituteRequest?> GetOpenSubRequestsForGame(string gameId)
    {
        var request = await dbContext.SubstituteRequests.FirstOrDefaultAsync(sr => sr.GameId == gameId);
        return request;
    }
}