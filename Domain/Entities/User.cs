using Domain.Primitives;

namespace Domain.Entities;

public sealed class User : Entity
{
    public User(Guid id, string email, string passwordHash) : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
    }

    private User() { }

    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
} 