namespace Cooklyn.Server.Domain.BuildInfo.Dtos;

public sealed record BuildInfoDto
{
    public string CommitSha { get; init; } = "";
    public string ShortSha { get; init; } = "";
    public string Branch { get; init; } = "";
    public string BuildTimestamp { get; init; } = "";
    public string RepositoryUrl { get; init; } = "";
    public List<CommitDto> Commits { get; init; } = [];
}

public sealed record CommitDto
{
    public string Sha { get; init; } = "";
    public string ShortSha { get; init; } = "";
    public string Message { get; init; } = "";
    public string Author { get; init; } = "";
    public string Date { get; init; } = "";
}
