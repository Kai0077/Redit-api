using Microsoft.EntityFrameworkCore;
using Redit_api.Data;
using Redit_api.Models;
using Redit_api.Repositories.Interfaces;

namespace Redit_api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDBContext _db;
        public UserRepository(AppDBContext db) => _db = db;

        public Task<bool> UsernameExistsAsync(string username, CancellationToken ct) =>
            _db.Users.AsNoTracking().AnyAsync(u => u.Username == username, ct);

        public Task<bool> EmailExistsAsync(string email, CancellationToken ct) =>
            _db.Users.AsNoTracking().AnyAsync(u => u.Email == email, ct);

        public Task<UserDTO?> GetByUsernameAsync(string username, CancellationToken ct) =>
            _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username, ct);

        public Task<UserDTO?> GetByEmailAsync(string email, CancellationToken ct) => 
            _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);

        public async Task<UserDTO> CreateAsync(UserDTO user, CancellationToken ct)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);
            return user;
        }
        
        public async Task UpdateAsync(UserDTO user, CancellationToken ct)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync(ct);
        }
    }
}