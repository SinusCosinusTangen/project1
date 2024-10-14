using project1.Models.DTO;
using project1.Models;

namespace project1.Services
{
    public interface IAuthService
    {
        CryptoDTO GetPublicKey();
        Task<UserDTO> RegisterUser(UserRequest request);
        Task<AuthDTO> Login(UserRequest userRequest);
        Task<UserDTO> UpdateUser(UserRequest request);
        Task DeleteUser(UserRequest request);
        Task<User> FindUserByUsername(string username);
        CryptoDTO GetEncryptedPassword(string password);
    }
}