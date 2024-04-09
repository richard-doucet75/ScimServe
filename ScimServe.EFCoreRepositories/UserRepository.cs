using AsyncAuthFlowCore.Abstractions;
using Microsoft.EntityFrameworkCore;
using ScimServe.Repositories;
using ScimServe.Repositories.Entities;

namespace ScimServe.EFCoreRepositories;

public class UserRepository : IUserRepository, IUserPermissionsRepository
{
    private readonly ScimDbContext _context;

    public UserRepository(ScimDbContext context)
    {
        _context = context;
    }

    public async Task<UserEntity?> FindLatestByIdentifierAsync(string identifier)
    {
        return await _context.Users.AsNoTracking()
            .OrderByDescending(u => u.Version)
            .FirstOrDefaultAsync(u => u.Id == identifier);
    }

    public async Task<bool> InsertAsync(UserEntity newUser)
    {
        await _context.Users.AddAsync(newUser);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<UserEntity?> FindByUsernameAsync(string username)
    {
        return await _context.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == username && !u.IsDeleted);
    }

    public async Task<bool> DatabaseIsSeedableForUsers()
    {
        return !await _context.Users.AnyAsync();
    }

    public async Task<DateTimeOffset> GetDateCreated(string identifier)
    {
        var user = await _context.Users.AsNoTracking()
            .FirstAsync(u => u.Id == identifier && u.Version == 1);
        
        return user.RecordTime;
    }

    public async Task<bool> VerifyUserPermission(string userId, string permissionRequired, CancellationToken cancellationToken)
    {
        return await _context.Users
            .AnyAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken: cancellationToken);
    }
}