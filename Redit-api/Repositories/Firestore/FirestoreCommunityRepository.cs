using Google.Cloud.Firestore;
using Redit_api.Models;
using Redit_api.Models.DTO;
using Redit_api.Models.Status;
using Redit_api.Repositories.Firestore.Interfaces;

namespace Redit_api.Repositories.Firestore;

public class FirestoreCommunityRepository : IFirestoreCommunityRepository
{
    private readonly FirestoreDb _db;
    public FirestoreCommunityRepository(FirestoreDb db)
    {
        _db = db;
    }
    
    public async Task<bool> ExistsAsync(string name, CancellationToken ct)
    {
        var communityRef = _db.Collection("community").WhereEqualTo("name", name);
        var communitySnapshot = await communityRef.GetSnapshotAsync(ct);

        return communitySnapshot.Count != 0;
    }

    public async Task<CommunityDTO?> GetAsync(string name, CancellationToken ct)
    {
        var communityRef = _db.Collection("community").WhereEqualTo("name", name);
        var communitySnapshot = await communityRef.GetSnapshotAsync(ct);

        if (communitySnapshot.Count == 0) throw new Exception($"Community with given name: {name} does not exist");

        var documentSnapshot = communitySnapshot.Documents[0];
        var community = documentSnapshot.ConvertTo<CommunityDTO>();

        return community;
    }

    public async Task<List<CommunityDTO>> ListAsync(int skip, int take, CancellationToken ct)
    {
        var communityRef =  _db.Collection("community");
        var query = communityRef.OrderBy("name").Limit(take);
        if (skip > 0)
        {
            var previousSnapshot = await communityRef.OrderBy("name").Limit(skip).GetSnapshotAsync(ct);
            
            var lastDoc = previousSnapshot.Documents.LastOrDefault();
            if (lastDoc != null)
            {
                query = query.StartAfter(lastDoc);
            }
        }

        var snapshot = await query.GetSnapshotAsync(ct);
        
        var communities = snapshot.Documents.Select(doc => doc.ConvertTo<CommunityDTO>()).ToList();

        return communities;
    }

    public async Task<CommunityDTO> CreateAsync(CommunityDTO community, CancellationToken ct)
    {
        var communityRef = await _db.Collection("community").AddAsync(community, ct);
       
        Console.WriteLine($"Added new community: {community.Name}");
        
        var communitySnapshot = await communityRef.GetSnapshotAsync(ct);
        var communityResponse = communitySnapshot.ConvertTo<CommunityDTO>();

        return communityResponse;
    }

    public async Task<CommunityDTO> UpdateAsync(CommunityDTO community, CancellationToken ct)
    {
        var communityRef = _db.Collection("community").WhereEqualTo("name", community.Name);
        var communitySnapshot = await communityRef.GetSnapshotAsync(ct);

        if (communitySnapshot.Count == 0) throw new Exception($"Community with given name: \"{community.Name}\" does not exist");
        
        var documentSnapshot = communitySnapshot.Documents[0];
        var documentRef = documentSnapshot.Reference;
        await documentRef.SetAsync(community, cancellationToken: ct);
        
        Console.WriteLine($"Updated document \"{community}\" on collection \"community\"");

        var newDocumentSnapshot = await documentRef.GetSnapshotAsync(ct);
        var newDocument =  newDocumentSnapshot.ConvertTo<CommunityDTO>();
        
        return newDocument;
    }

    public async Task<bool> DeleteAsync(string name, CancellationToken ct)
    {
        var communityRef = _db.Collection("community").WhereEqualTo("name", name);
        var communitySnapshot = await communityRef.GetSnapshotAsync(ct);
        
        if (communitySnapshot.Count == 0) throw new Exception($"Document with given name: {name} does not exist");

        var documentSnapshot = communitySnapshot.Documents[0];
        var documentRef = documentSnapshot.Reference;
        
        await documentRef.DeleteAsync(cancellationToken: ct);

        return true;
    }

    public async Task<bool> UserExistsAsync(string username, CancellationToken ct)
    {
        var userRef = _db.Collection("user").Document(username);
        var userSnapshot = await userRef.GetSnapshotAsync(ct);

        return userSnapshot.Exists;
    }

    public async Task<string?> GetUsernameByEmailAsync(string email, CancellationToken ct)
    {
        var userRef = _db.Collection("user").WhereEqualTo("email", email);
        var userSnapshot = await userRef.GetSnapshotAsync(ct);
        
        if (userSnapshot.Count == 0) throw new Exception($"User with given email: {email} does not exist");
        
        var documentSnapshot = userSnapshot.Documents[0];
        var user = documentSnapshot.ConvertTo<UserDTO>();

        return user.Username;
    }

    public async Task<UserRole?> GetRoleByEmailAsync(string email, CancellationToken ct)
    {
        var userRef = _db.Collection("user").WhereEqualTo("email", email);
        var userSnapshot = await userRef.GetSnapshotAsync(ct);
        
        if (userSnapshot.Count == 0) throw new Exception($"User with given email: {email} does not exist");
        
        var documentSnapshot = userSnapshot.Documents[0];
        var user = documentSnapshot.ConvertTo<UserDTO>();

        return user.Role;
    }

    public async Task<List<CommunityDTO>> GetAllAsync(CancellationToken ct)
    {
        var communityRef = _db.Collection("community");
        var communitySnapshot = await communityRef.GetSnapshotAsync(ct);
        
        var communities = communitySnapshot.Documents.Select(doc =>  doc.ConvertTo<CommunityDTO>()).ToList();

        return communities;
    }

    public async Task<List<CommunityDTO>> GetByUserAsync(string username, CancellationToken ct)
    {
        var communityRef = _db.Collection("community").WhereEqualTo("owner_username", username);
        var communitySnapshot = await communityRef.GetSnapshotAsync(ct);
        
        var communities = communitySnapshot.Documents.Select(doc =>  doc.ConvertTo<CommunityDTO>()).ToList();

        return communities;
    }
}