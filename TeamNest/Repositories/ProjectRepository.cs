using Dapper;
using TeamNest.Data;
using TeamNest.Models.Dtos;
using TeamNest.Models.Entities;
using TeamNest.Services;

namespace TeamNest.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly ISqlConnectionFactory _db;
    private readonly ICurrentUserService _currentUser;

    public ProjectRepository(ISqlConnectionFactory db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    // MULTI-TENANCY: the tenant id always comes from the login cookie, never from the browser
    private int OrganizationId => _currentUser.OrganizationId;

    private const string ListColumns = @"
        p.Id, p.Name, p.Description, p.Status, p.StartDate, p.DueDate, p.CreatedAt,
        u.FullName AS CreatedByName";

    public async Task<IEnumerable<ProjectListItem>> SearchAsync(string? term, ProjectStatus? status)
    {
        var sql = $@"
            SELECT {ListColumns}
            FROM Projects p
            LEFT JOIN AspNetUsers u ON u.Id = p.CreatedByUserId
            WHERE p.OrganizationId = @OrganizationId
              AND p.IsDeleted = 0
              AND (@Term IS NULL OR p.Name LIKE @Term OR p.Description LIKE @Term)
              AND (@Status IS NULL OR p.Status = @Status)
            ORDER BY p.CreatedAt DESC;";

        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<ProjectListItem>(sql, new
        {
            OrganizationId,
            Term = string.IsNullOrWhiteSpace(term) ? null : $"%{term.Trim()}%",
            Status = (int?)status
        });
    }

    public async Task<IEnumerable<ProjectListItem>> GetRecentAsync(int count)
    {
        var sql = $@"
            SELECT TOP (@Count) {ListColumns}
            FROM Projects p
            LEFT JOIN AspNetUsers u ON u.Id = p.CreatedByUserId
            WHERE p.OrganizationId = @OrganizationId AND p.IsDeleted = 0
            ORDER BY p.CreatedAt DESC;";

        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<ProjectListItem>(sql, new { OrganizationId, Count = count });
    }

    public async Task<Project?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT Id, OrganizationId, Name, Description, Status, StartDate, DueDate,
                   CreatedByUserId, CreatedAt, IsDeleted
            FROM Projects
            WHERE Id = @Id AND OrganizationId = @OrganizationId AND IsDeleted = 0;";

        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Project>(sql, new { Id = id, OrganizationId });
    }

    public async Task<int> CreateAsync(Project project)
    {
        const string sql = @"
            INSERT INTO Projects
                (OrganizationId, Name, Description, Status, StartDate, DueDate, CreatedByUserId, CreatedAt, IsDeleted)
            OUTPUT INSERTED.Id
            VALUES
                (@OrganizationId, @Name, @Description, @Status, @StartDate, @DueDate, @CreatedByUserId, @CreatedAt, 0);";

        using var connection = _db.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, new
        {
            OrganizationId,                 // forced: ignore whatever was on the object
            project.Name,
            project.Description,
            Status = (int)project.Status,
            project.StartDate,
            project.DueDate,
            project.CreatedByUserId,
            project.CreatedAt
        });
    }

    public async Task<bool> UpdateAsync(Project project)
    {
        const string sql = @"
            UPDATE Projects
            SET Name = @Name,
                Description = @Description,
                Status = @Status,
                StartDate = @StartDate,
                DueDate = @DueDate
            WHERE Id = @Id AND OrganizationId = @OrganizationId AND IsDeleted = 0;";

        using var connection = _db.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new
        {
            project.Id,
            OrganizationId,
            project.Name,
            project.Description,
            Status = (int)project.Status,
            project.StartDate,
            project.DueDate
        });
        return rows > 0;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        const string sql = @"
            UPDATE Projects SET IsDeleted = 1
            WHERE Id = @Id AND OrganizationId = @OrganizationId AND IsDeleted = 0;";

        using var connection = _db.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { Id = id, OrganizationId });
        return rows > 0;
    }
}
