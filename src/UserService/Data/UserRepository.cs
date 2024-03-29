using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task AddUser(User user)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User> GetUserByLogin(string login)
    {
        return (await _dbContext
            .Users
            .FirstOrDefaultAsync(u => u.Login == login))!;
    }
    
    public bool UserExists(string login)
    {
        return _dbContext.Users.Any(u => u.Login == login);
    }
}