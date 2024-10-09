namespace project1.Models.DTO
{
    public class UserDTO
    {
        public required string Email { get; set;}
        public required string Username { get; set;}
        public required string Role { get; set;}
        public required string LoginMethod { get; set;}
        public DateTime LastLoggedOn { get; set;}
    }
}