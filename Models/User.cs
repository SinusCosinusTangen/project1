using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project1.Models
{
    [Table("user_auth")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set;}
        [Column("email")]
        public required string Email { get; set;}
        [Column("username")]
        public required string Username { get; set;}
        [Column("password")]
        public required string Password { get; set;}
        [Column("role")]
        public required string Role { get; set;}
        [Column("login_method")]
        public required string LoginMethod { get; set;}
        [Column("last_logged_on")]
        public DateTime LastLoggedOn { get; set;}
        [Column("created_date")]
        public DateTime CreatedDate { get; set;}
        [Column("last_modified")]
        public DateTime LastModified { get; set;}
    }
}