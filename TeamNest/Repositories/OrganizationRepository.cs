using Dapper;
using TeamNest.Data;
using TeamNest.Helpers;
using TeamNest.Models.Dtos;
using TeamNest.Models.Entities;

namespace TeamNest.Repositories;

/// <summary>All SQL for organizations, written with Dapper.</summary>
public class OrganizationRepository : IOrganizationRepository
{
    private readonly ISqlConnectionFactory _db;

    public OrganizationRepository(ISqlConnectionFactory db)
    {
        _db = db;
    }

    public async Task<int> CreateAsync(string name, string inviteCode)
    {
        const string sql = @"
            INSERT INTO Organizations (Name, InviteCode, CreatedAt)
            OUTPUT INSERTED.Id
            VALUES (@Name, @InviteCode, SYSUTCDATETIME());";

        using var connection = _db.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, new { Name = name, InviteCode = inviteCode });
    }

    public async Task<Organization?> GetByIdAsync(int id)
    {
        const string sql = "SELECT Id, Name, InviteCode, CreatedAt FROM Organizations WHERE Id = @Id;";

        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Organization>(sql, new { Id = id });
    }

    public async Task<Organization?> GetByInviteCodeAsync(string inviteCode)
    {
        const string sql = "SELECT Id, Name, InviteCode, CreatedAt FROM Organizations WHERE InviteCode = @InviteCode;";

        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Organization>(
            sql, new { InviteCode = InviteCodeGenerator.Normalize(inviteCode) });
    }

    public async Task<bool> InviteCodeExistsAsync(string inviteCode)
    {
        const string sql = "SELECT COUNT(1) FROM Organizations WHERE InviteCode = @InviteCode;";

        using var connection = _db.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            sql, new { InviteCode = InviteCodeGenerator.Normalize(inviteCode) });
        return count > 0;
    }

    public async Task<string> GenerateUniqueInviteCodeAsync()
    {
        // Collisions are extremely rare, but we still check
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var code = InviteCodeGenerator.Create();
            if (!await InviteCodeExistsAsync(code))
            {
                return code;
            }
        }

        throw new InvalidOperationException("Could not generate a unique invite code.");
    }

    public async Task UpdateInviteCodeAsync(int organizationId, string inviteCode)
    {
        const string sql = "UPDATE Organizations SET InviteCode = @InviteCode WHERE Id = @Id;";

        using var connection = _db.CreateConnection();
        await connection.ExecuteAsync(sql, new { Id = organizationId, InviteCode = inviteCode });
    }

    public async Task DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Organizations WHERE Id = @Id;";

        using var connection = _db.CreateConnection();
        await connection.ExecuteAsync(sql, new { Id = id });
    }

    public async Task<IEnumerable<TeamMemberDto>> GetMembersAsync(int organizationId)
    {
        // Joins our custom AspNetUsers columns with the Identity role tables
        const string sql = @"
            SELECT u.Id, u.FullName, u.Email, u.CreatedAt AS JoinedAt, r.Name AS Role
            FROM AspNetUsers u
            LEFT JOIN AspNetUserRoles ur ON ur.UserId = u.Id
            LEFT JOIN AspNetRoles r ON r.Id = ur.RoleId
            WHERE u.OrganizationId = @OrganizationId
            ORDER BY CASE r.Name WHEN 'Owner' THEN 0 WHEN 'Admin' THEN 1 ELSE 2 END, u.FullName;";

        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<TeamMemberDto>(sql, new { OrganizationId = organizationId });
    }

    public async Task<WorkspaceStats> GetStatsAsync(int organizationId)
    {
        // QueryMultiple: two SELECTs, one round trip to the database
        const string sql = @"
            SELECT COUNT(*) AS TotalProjects,
                   ISNULL(SUM(CASE WHEN Status = @Active THEN 1 ELSE 0 END), 0) AS ActiveProjects,
                   ISNULL(SUM(CASE WHEN Status = @Completed THEN 1 ELSE 0 END), 0) AS CompletedProjects
            FROM Projects
            WHERE OrganizationId = @OrganizationId AND IsDeleted = 0;

            SELECT COUNT(*) FROM AspNetUsers WHERE OrganizationId = @OrganizationId;";

        using var connection = _db.CreateConnection();
        using var results = await connection.QueryMultipleAsync(sql, new
        {
            OrganizationId = organizationId,
            Active = (int)ProjectStatus.Active,
            Completed = (int)ProjectStatus.Completed
        });

        var stats = await results.ReadSingleAsync<WorkspaceStats>();
        stats.MemberCount = await results.ReadSingleAsync<int>();
        return stats;
    }
}
