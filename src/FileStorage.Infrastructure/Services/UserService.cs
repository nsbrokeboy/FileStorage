using FileStorage.Application.Common.Exceptions;
using FileStorage.Application.Interfaces;
using FileStorage.Domain.Entities;
using FileStorage.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FileStorage.Infrastructure.Services;

/// <inheritdoc />
public class UserService : IUserService
{
    private readonly FileStorageDbContext _dbContext;

    public UserService(FileStorageDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<Guid> CreateUserAsync(CancellationToken token)
    {
        var user = new User();
        await _dbContext.Users.AddAsync(user, token);
        await _dbContext.SaveChangesAsync(token);
        
        return user.Id;
    }

    /// <inheritdoc />
    public async Task<User> GetUserByIdAsync(Guid userId, CancellationToken token)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == userId, token);
        if (user == null)
        {
            throw new NotFoundException("Пользователь не найден");
        }
        
        return user;
    }
}