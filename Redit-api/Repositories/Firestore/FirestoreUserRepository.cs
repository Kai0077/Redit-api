using Redit_api.Models;
using Redit_api.Repositories.Firestore.Interfaces;
using Google.Cloud.Firestore;

namespace Redit_api.Repositories.Firestore;

public class UserRepository : IUserRepository
{
    private readonly FirestoreDb _db;
    private readonly string _connectionString;
    public UserRepository(FirestoreDb db, string connectionString)
    {
        _db = db;
        _connectionString = connectionString;
    }
    
    public async Task<bool> UsernameExistsAsync(string username, CancellationToken ct)
    {
        var userReference = _db.Collection("user").Document(username);
        var userSnapshot = await userReference.GetSnapshotAsync(ct);

        return userSnapshot.Exists;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct)
    {
        var userReference = _db.Collection("user").WhereEqualTo("email", email);
        var userSnapshot = await userReference.GetSnapshotAsync(ct);

        return userSnapshot.Count > 0;
    }

    public async Task<UserDTO?> GetByUsernameAsync(string username, CancellationToken ct)
    {
        var userReference = _db.Collection("user").Document(username);
        var userSnapshot = await userReference.GetSnapshotAsync(ct);

        if (!userSnapshot.Exists) throw new Exception($"User with given username: {username} does not exists");
        
        UserDTO user =  userSnapshot.ConvertTo<UserDTO>();
        return user;

    }

    public async Task<UserDTO?> GetByEmailAsync(string email, CancellationToken ct)
    {
        var userReference = _db.Collection("user").WhereEqualTo("email", email);
        var userSnapshot = await userReference.GetSnapshotAsync(ct);
        
        if (userSnapshot.Count == 0) throw new Exception($"User with given email: {email} does not exists");

        var documentSnapshot = userSnapshot.Documents[0];
        var user =  documentSnapshot.ConvertTo<UserDTO>();

        return user;
    }

    public async Task<UserDTO> CreateAsync(UserDTO user, CancellationToken ct)
    {
        var userRef = await _db.Collection("user").AddAsync(user, ct);
        
        Console.WriteLine($"Added document \"{user}\"on collection \"user\"");

        var userSnapshot = await userRef.GetSnapshotAsync(ct);
        var userResponse = userSnapshot.ConvertTo<UserDTO>();

        return userResponse;
    }

    public async Task UpdateAsync(UserDTO user, CancellationToken ct)
    {
        var username = user.Username;
        var userRef = _db.Collection("user").Document(username);
        
        await userRef.SetAsync(user, cancellationToken: ct);
        
        Console.WriteLine($"Updated document \"{user}\" on collection \"user\"");
    }

    public async Task<List<UserDTO>> GetAllAsync(CancellationToken ct)
    {
        var usersRef = _db.Collection("user");
        var usersSnapshot = await usersRef.GetSnapshotAsync(ct);
        
        var users = usersSnapshot.Documents.Select(doc => doc.ConvertTo<UserDTO>()).ToList();

        return users;
    }

    public async Task DeleteUserAsync(string username, CancellationToken ct)
    {
        var userRef = _db.Collection("user").Document(username);
        await userRef.DeleteAsync(cancellationToken: ct);
    }

    public async Task<List<UserDTO>> GetFollowersAsync(string username, CancellationToken ct)
    {
        var followersReference = _db.Collection("user_followers").WhereEqualTo("target_username", username);
        var followersSnapshot = await followersReference.GetSnapshotAsync(ct);
        
        var followerUsernames = followersSnapshot.Documents.Select(doc => doc.ConvertTo<Dictionary<string, string>>()["follower_username"]).ToList();

        var followerUsers = new List<UserDTO>();
        
        const int batchSize = 30; //Firestore WhereIn limit
        
        for (var i = 0; i < followerUsernames.Count; i += batchSize)
        {
            var batch = followerUsernames.Skip(i).Take(batchSize).ToList();
            var userQuery = _db.Collection("user").WhereIn("username", batch);
            var userSnapshots = await userQuery.GetSnapshotAsync(ct);
            
            followerUsers.AddRange(userSnapshots.Documents.Select(doc => doc.ConvertTo<UserDTO>()));
        }

        return followerUsers;
    }

    public async Task<List<UserDTO>> GetFollowingAsync(string username, CancellationToken ct)
    {
        var followingReference = _db.Collection("user_follows").WhereEqualTo("follower_username", username);
        var followingSnapshot = await followingReference.GetSnapshotAsync(ct);
        
        var followingUsernames = followingSnapshot.Documents.Select(doc => doc.ConvertTo<Dictionary<string, string>>()["following_username"]).ToList();
        
        var followingUsers = new List<UserDTO>();

        const int batchSize = 30; //Firestore WhereIn limit

        for (var i = 0; i < followingUsernames.Count; i += batchSize)
        {
            var batch = followingUsernames.Skip(i).Take(batchSize).ToList();
            var userQuery = _db.Collection("user").WhereIn("username", batch);
            var userSnapshots = await userQuery.GetSnapshotAsync(ct);
            
            followingUsers.AddRange(userSnapshots.Documents.Select(doc => doc.ConvertTo<UserDTO>()));
        }
        
        return followingUsers;
    }

    public async Task<List<string>> GetFollowerUsernamesAsync(string username, CancellationToken ct)
    {
        var followingReference = _db.Collection("user_followers").WhereEqualTo("target_username", username);
        var followingSnapshot = await followingReference.GetSnapshotAsync(ct);
        
        var followingUsernames = followingSnapshot.Documents.Select(doc => doc.ConvertTo<Dictionary<string, string>>()["follower_username"]).ToList();

        return followingUsernames;
    }

    public async Task<List<string>> GetFollowingUsernamesAsync(string username, CancellationToken ct)
    {
        var followingReference = _db.Collection("user_follows").WhereEqualTo("follower_username", username);
        var followingSnapshot = await followingReference.GetSnapshotAsync(ct);
        
        var followingUsernames = followingSnapshot.Documents.Select(doc => doc.ConvertTo<Dictionary<string, string>>()["following_username"]).ToList();

        return followingUsernames;
    }
}