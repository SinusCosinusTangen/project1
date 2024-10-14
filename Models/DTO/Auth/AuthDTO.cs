using NuGet.Common;

namespace project1.Models.DTO
{
    public class AuthDTO
    {
        public string Email { get; set;}
        public string Username { get; set;}
        public string Role { get; set;}
        public string Token { get; set;}

        public AuthDTO(User user, string token)
        {
            Email = user.Email;
            Username = user.Username;
            Role = user.Role;
            Token = token;
        }
    }

}