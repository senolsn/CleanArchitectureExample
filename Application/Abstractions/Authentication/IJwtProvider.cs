namespace Application.Abstractions.Authentication;

public interface IJwtProvider
{
    string Generate(Guid userId, string email);
} 