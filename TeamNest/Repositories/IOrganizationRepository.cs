using TeamNest.Models.Dtos;
using TeamNest.Models.Entities;

namespace TeamNest.Repositories;

public interface IOrganizationRepository
{
    Task<int> CreateAsync(string name, string inviteCode);
    Task<Organization?> GetByIdAsync(int id);
    Task<Organization?> GetByInviteCodeAsync(string inviteCode);
    Task<bool> InviteCodeExistsAsync(string inviteCode);
    Task<string> GenerateUniqueInviteCodeAsync();
    Task UpdateInviteCodeAsync(int organizationId, string inviteCode);
    Task DeleteAsync(int id);

    Task<IEnumerable<TeamMemberDto>> GetMembersAsync(int organizationId);
    Task<WorkspaceStats> GetStatsAsync(int organizationId);
}
