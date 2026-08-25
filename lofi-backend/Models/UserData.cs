using System.ComponentModel.DataAnnotations;
using lofi_backend.Data_Models.Enums;

namespace lofi_backend.Models
{
    public class UserData
    {
        public UserData() { }

        public UserData(string id, string username, string firstName, string lastName, string email, DateTime dateOfBirth, Gender gender)
        {
            Id = id;
            Username = username;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            DateOfBirth = dateOfBirth;
            Gender = gender;
        }

        public string Id { get; set; } = string.Empty;
        [Required]
        public string Username { get; set; } 
        [Required]
        public string FirstName { get; set; } 
        [Required]
        public string LastName { get; set; } 
        [Required]
        public string Email { get; set; } 
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public Gender Gender { get; set; }

        public bool IsAdmin { get; set; } = false;

        public List<Playlist> Playlists { get; set; } = new List<Playlist>();
    }

    
    public class  UserWithPassword
    {
        public UserWithPassword() { }

        public UserWithPassword(UserData user, string password)
        {
            UserData = user;
            Password = password;
        }

        public UserData UserData { get; set; }
        public string Password { get; set; }
    }
}
