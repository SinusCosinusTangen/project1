namespace project1.Models.DTO
{
    public class UserRequest
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? NewPassword { get; set; }
        public string? LoginMethod { get; set; }
    }
}