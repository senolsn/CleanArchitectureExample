using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await _dbContext.Set<User>()
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

    public void Insert(User user) => _dbContext.Set<User>().Add(user);
} 