using Google.Cloud.Firestore;
using Redit_api.Models;
using Redit_api.Models.DTO;
using Redit_api.Repositories.Firestore.Interfaces;

namespace Redit_api.Repositories.Firestore;

public class FirestoreCommentRepository : IFirestoreCommentRepository
{
    private readonly FirestoreDb _db;
    public FirestoreCommentRepository(FirestoreDb db)
    {
        _db = db;
    }
    
    public async Task<CommentDTO> CreateAsync(CommentDTO comment, CancellationToken ct)
    {
        var commentRef = await _db.Collection("comments").AddAsync(comment, ct);
        
        Console.WriteLine($"Added new comment: {comment.Id} on post: {comment.PostId}");
        
        var commentSnapshot = await commentRef.GetSnapshotAsync(ct);
        var commentResponse = commentSnapshot.ConvertTo<CommentDTO>();

        return commentResponse;
    }

    public async Task<CommentDTO?> GetByIdAsync(int id, CancellationToken ct)
    {
        var commentRef = _db.Collection("comments").Document(id.ToString());
        var commentSnapshot = await commentRef.GetSnapshotAsync(ct);
        
        if (!commentSnapshot.Exists) throw new Exception("Comment not found");
        
        var comment =  commentSnapshot.ConvertTo<CommentDTO>();
        
        return comment;
    }

    public async Task UpdateAsync(CommentDTO comment, CancellationToken ct)
    {
        var  commentRef = _db.Collection("comments").Document(comment.Id.ToString());
        await commentRef.SetAsync(comment, cancellationToken: ct);
        
        Console.WriteLine($"Updated comment: {comment.Id} on post: {comment.PostId}");
    }

    public async Task DeleteAsync(CommentDTO comment, CancellationToken ct)
    {
        var commentRef = _db.Collection("comments").Document(comment.Id.ToString());
        await commentRef.DeleteAsync(cancellationToken: ct);
    }

    public async Task<bool> PostExistsAsync(int postId, CancellationToken ct)
    {
        var postRef = _db.Collection("posts").Document(postId.ToString());
        var postSnapshot = await postRef.GetSnapshotAsync(ct);
        
        return postSnapshot.Exists;
    }

    public async Task<bool> ParentBelongsToPostAsync(int parentId, int postId, CancellationToken ct)
    {
        var commentsRef = _db.Collection("comments");
        var query = commentsRef.WhereEqualTo("id", parentId).WhereEqualTo("post_id", postId);
        
        var snapshot = await query.GetSnapshotAsync(ct);

        return snapshot.Count > 0;
    }

    public async Task<string?> GetUsernameByEmailAsync(string email, CancellationToken ct)
    {
        var userRef = _db.Collection("users").WhereEqualTo("email", email);
        var userSnapshot = await userRef.GetSnapshotAsync(ct);

        if (userSnapshot.Count == 0) throw new Exception($"No user with given email: {email} found");

        var userDocumentSnapshot = userSnapshot.Documents[0];
        var user = userDocumentSnapshot.ConvertTo<User>();

        return user.Name;
    }

    public async Task<List<CommentDTO>> GetByPostAsync(int postId, int skip, int take, CancellationToken ct)
    {
        var commentsRef = _db.Collection("comments").WhereEqualTo("post_id", postId).OrderBy("id").Limit(take);

        if (skip > 0)
        {
            var previousSnapshot = await _db.Collection("comments").WhereEqualTo("post_id", postId).OrderBy("id")
                .Limit(skip).GetSnapshotAsync(ct);

            var lastDoc = previousSnapshot.Documents.LastOrDefault();
            if (lastDoc != null)
            {
                commentsRef = commentsRef.StartAfter(lastDoc);
            }
        }

        var snapshot = await commentsRef.GetSnapshotAsync(ct);

        var comments = snapshot.Documents.Select(doc => doc.ConvertTo<CommentDTO>()).ToList();

        return comments;
    }

    public async Task<List<CommentDTO>> GetByUserAsync(string username, int skip, int take, CancellationToken ct)
    {
        var commentsRef = _db.Collection("comments").WhereEqualTo("commenter", username).OrderBy("id").Limit(take);

        if (skip > 0)
        {
            var previousSnapshot = await _db.Collection("comments").WhereEqualTo("commenter", username).OrderBy("id").Limit(skip).GetSnapshotAsync(ct);
            
            var lastDoc = previousSnapshot.Documents.LastOrDefault();
            if (lastDoc != null)
            {
                commentsRef = commentsRef.StartAfter(lastDoc);
            }
        }

        var snapshot = await commentsRef.GetSnapshotAsync(ct);
        
        var comments = snapshot.Documents.Select(doc => doc.ConvertTo<CommentDTO>()).ToList();

        return comments;
    }
}