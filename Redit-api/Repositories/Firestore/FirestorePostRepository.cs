using Google.Cloud.Firestore;
using Redit_api.Models;
using Redit_api.Repositories.Firestore.Interfaces;

namespace Redit_api.Repositories.Firestore;

public class FirestorePostRepository : IFirestorePostRepository
{
    private readonly FirestoreDb _db;

    public FirestorePostRepository(FirestoreDb db)
    {
        _db = db;
    }
    
    public async Task<PostDTO> CreateAsync(PostDTO post, CancellationToken ct)
    {
        var postRef = await _db.Collection("post").AddAsync(post, ct);
        
        Console.WriteLine($"Added document \"{post}\" on collection \"post\"");

        var postSnapshot = await postRef.GetSnapshotAsync(ct);
        var postResponse = postSnapshot.ConvertTo<PostDTO>();

        return postResponse;
    }

    public async Task<PostDTO?> GetByIdAsync(int id, CancellationToken ct)
    {
        var postReference = _db.Collection("post").Document(id.ToString());
        var postSnapshot = await postReference.GetSnapshotAsync(ct);

        if (!postSnapshot.Exists) throw new Exception($"Post with given id: {id} does not exist");

        var post = postSnapshot.ConvertTo<PostDTO>();
        return post;
    }

    public async Task UpdateAsync(PostDTO post, CancellationToken ct)
    {
        var postId = post.Id;
        var postRef = _db.Collection("post").Document(postId.ToString());

        await postRef.SetAsync(post, cancellationToken: ct);
        
        Console.WriteLine($"Updated document \"{post}\" on collection \"post\"");
    }

    public async Task DeleteAsync(PostDTO post, CancellationToken ct)
    {
        var postId = post.Id;
        var postRef = _db.Collection("post").Document(postId.ToString());

        await postRef.DeleteAsync(cancellationToken: ct);
    }

    public async Task<bool> CommunityExistsAsync(string communityName, CancellationToken ct)
    {
        var communityRef = _db.Collection("community").WhereEqualTo("name", communityName);
        var communitySnapshot = await communityRef.GetSnapshotAsync(ct);

        return communitySnapshot.Count != 0;
    }

    public async Task<string?> GetUsernameByEmailAsync(string email, CancellationToken ct)
    {
        var userRef = _db.Collection("user").WhereEqualTo("email", email);
        var userSnapshot = await userRef.GetSnapshotAsync(ct);

        if (userSnapshot.Count == 0) throw new Exception($"User with email {email} does not exist");

        var documentSnapshot = userSnapshot.Documents[0];
        var user = documentSnapshot.ConvertTo<UserDTO>();
        
        return user?.Username;
    }

    public async Task<UserDTO?> GetUserByEmailAsync(string email, CancellationToken ct)
    {
        var userRef = _db.Collection("user").WhereEqualTo("email", email);
        var userSnapshot = await userRef.GetSnapshotAsync(ct);

        if (userSnapshot.Count == 0) throw new Exception($"User with email {email} does not exist");

        var documentSnapshot = userSnapshot.Documents[0];
        var user = documentSnapshot.ConvertTo<UserDTO>();
        
        return user;
    }

    public async Task<IEnumerable<PostDTO>> GetAllPublicAsync(CancellationToken ct)
    {
        var postReference = _db.Collection("post").WhereEqualTo("is_public", true);
        var postSnapshot = await postReference.GetSnapshotAsync(ct);
        
        var posts = postSnapshot.Documents.Select(document => document.ConvertTo<PostDTO>()).ToList();
        
        return posts;
    }

    public async Task<IEnumerable<PostDTO>> GetAllAsync(CancellationToken ct)
    {
        var postReference = _db.Collection("post");
        var postSnapshot = await postReference.GetSnapshotAsync(ct);

        var posts = postSnapshot.Documents.Select(document => document.ConvertTo<PostDTO>()).ToList();

        return posts;
    }

    public async Task<IEnumerable<PostDTO>> GetByUserAsync(string username, CancellationToken ct)
    {
        var postRef = _db.Collection("post").WhereEqualTo("original_poster", username);
        var postSnapshot = await postRef.GetSnapshotAsync(ct);
        
        var posts = postSnapshot.Documents.Select(document => document.ConvertTo<PostDTO>()).ToList();

        return posts;
    }
}