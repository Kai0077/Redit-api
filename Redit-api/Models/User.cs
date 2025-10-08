namespace Redit_api.Models
{
    
    public class User
    {
       public string Name { get; set; }
       public string Username { get; set; }
       public string Email { get; set; }
       public int Age { get; set; }
       public string PasswordHash { get; set; }
       public int Aura { get; set; }
       public Comment[] Comments { get; set; }
       public Post[] Posts { get; set; }
       public Community[] Communities { get; set; }
       public User[] Followers { get; set; }
       public User[] Following { get; set; }
       public string ProfilePicture { get; set; }
       public Status AccountStatus { get; set; }
       public string Bio { get; set; }
       public Community[] Owns { get; set; }
       public Community[] Administrates { get; set; }
       public Community[] Moderates { get; set; }
    }
}
