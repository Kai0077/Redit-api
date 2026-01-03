using Redit_api.Models;

namespace Redit_api.Repositories.Neo4j.Interfaces
{
    public interface INeo4jUserReadRepository
    {
        Task<List<UserDTO>> GetAllAsync(CancellationToken ct);
    }
}