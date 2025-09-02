using TheBench.Logic.Models;

namespace TheBench.Logic.Adapters;

public interface INotificationAdapter
{
    public Task<bool> SendTeamInvite(TeamInvitation teamInvitation);
}