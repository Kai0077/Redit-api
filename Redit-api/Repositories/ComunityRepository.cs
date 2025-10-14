// Repositories/CommunityRepository.cs
using Microsoft.EntityFrameworkCore;
using Redit_api.Data;
using Redit_api.Models;
using Redit_api.Models.DTO;
using Redit_api.Models.Status;
using Redit_api.Repositories.Interfaces;

namespace Redit_api.Repositories
{
    public class CommunityRepository : ICommunityRepository
    {
        private readonly AppDBContext _db;
        public CommunityRepository(AppDBContext db) => _db = db;

        public Task<bool> ExistsAsync(string name, CancellationToken ct) =>
            _db.Communities.AsNoTracking().AnyAsync(c => c.Name == name, ct);

        public Task<CommunityDTO?> GetAsync(string name, CancellationToken ct) =>
            _db.Communities.AsNoTracking().FirstOrDefaultAsync(c => c.Name == name, ct);

        public async Task<List<CommunityDTO>> ListAsync(int skip, int take, CancellationToken ct) =>
            await _db.Communities.AsNoTracking().OrderBy(c => c.Name).Skip(skip).Take(take).ToListAsync(ct);

        public async Task<CommunityDTO> CreateAsync(CommunityDTO community, CancellationToken ct)
        {
            _db.Communities.Add(community);
            await _db.SaveChangesAsync(ct);
            return community;
        }

        public async Task<CommunityDTO> UpdateAsync(CommunityDTO community, CancellationToken ct)
        {
            _db.Communities.Update(community);
            await _db.SaveChangesAsync(ct);
            return community;
        }

        public async Task<bool> DeleteAsync(string name, CancellationToken ct)
        {
            var entity = await _db.Communities.FirstOrDefaultAsync(c => c.Name == name, ct);
            if (entity == null) return false;
            _db.Communities.Remove(entity);
            await _db.SaveChangesAsync(ct);
            return true;
        }
        
        public async Task<List<CommunityDTO>> GetAllAsync(CancellationToken ct)
        {
            return await _db.Communities
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync(ct);
        }

        public async Task<List<CommunityDTO>> GetByUserAsync(string username, CancellationToken ct)
        {
            // Gets all communities linked to a user — both ones they own and ones they’ve joined.
            // Using raw SQL here because it’s easier to do the UNION this way than with LINQ.
            return await _db.Communities
                .FromSqlRaw(@"
                    SELECT c.*
                    FROM community c
                    WHERE c.owner_username = {0}
                    UNION
                    SELECT c.*
                    FROM community c
                    JOIN user_communities uc ON uc.community_name = c.name
                    WHERE uc.username = {0}", username)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public Task<string?> GetUsernameByEmailAsync(string email, CancellationToken ct) =>
            _db.Users.Where(u => u.Email == email)
                .Select(u => u.Username)
                .FirstOrDefaultAsync(ct);
        
        public Task<bool> UserExistsAsync(string username, CancellationToken ct) =>
            _db.Users.AsNoTracking().AnyAsync(u => u.Username == username, ct);

        
        public Task<UserRole?> GetRoleByEmailAsync(string email, CancellationToken ct) =>
            _db.Users.AsNoTracking().Where(u => u.Email == email).Select(u => (UserRole?)u.Role).FirstOrDefaultAsync(ct);
    }
}