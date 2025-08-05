using TheBench.Logic.Models;

namespace TheBench.Logic.Adapters;

public interface ISubstituteRequestAdapter
{
    public Task<SubstituteRequest> CreateSubstituteRequest(SubstituteRequest substituteRequest);
    public Task<List<SubstituteRequest>> GetOpenSubstituteRequests();
    public Task<SubstituteRequest?> GetOpenSubRequestsForGame(string gameId);
}