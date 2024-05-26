namespace API.Models.Authentication;

public record AuthResponse
{
    public string Token { get; init; } = default!;

    public DateTime CreatedAt { get; init; }
}