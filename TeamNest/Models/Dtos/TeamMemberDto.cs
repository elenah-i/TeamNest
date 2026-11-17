namespace TeamNest.Models.Dtos;

public class TeamMemberDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Role { get; set; }
    public DateTime JoinedAt { get; set; }

    public string Initials =>
        string.Concat(FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                              .Take(2)
                              .Select(part => char.ToUpperInvariant(part[0])));
}
