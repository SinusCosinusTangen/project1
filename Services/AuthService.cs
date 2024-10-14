using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using project1.Exceptions;
using project1.Models;
using project1.Models.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace project1.Services
{
	public class AuthService(IOptions<AppSettings> appSettings, UserContext userContext, ICryptoService cryptoService) : IAuthService
    {
        private readonly AppSettings _appSettings = appSettings.Value;
        private readonly UserContext _context = userContext;
		private readonly ICryptoService _cryptoService = cryptoService;

		public CryptoDTO GetEncryptedPassword(string password)
		{
			CryptoDTO cryptoDTO = new()
			{
				publicKey = _cryptoService.EncryptMessage(_cryptoService.GetPublicKey(), password)
			};
			return cryptoDTO;
		}

		public CryptoDTO GetPublicKey()
		{
            CryptoDTO cryptoDTO = new()
            {
                publicKey = _cryptoService.GetPublicKey()
            };

            return cryptoDTO;
		}

        public async Task<UserDTO> RegisterUser(UserRequest request)
        {
			if (string.IsNullOrEmpty(request.Username) && string.IsNullOrEmpty(request.Email))
			{
				throw new ArgumentException("USERNAME OR EMAIL IS EMPTY");
			}
			else if (string.IsNullOrEmpty(request.Password))
			{
				throw new ArgumentException("PASSWORD IS EMPTY");
			}

			User existingUser = await _context.Users
				.FirstOrDefaultAsync(user =>
					(request.Username != null && (request.Username.Equals(user.Username))) ||
					(request.Email != null && request.Email.Equals(user.Email)));

			if (existingUser != null)
			{
				throw new UserExistsException();
			}

			User user = new() 
			{
				Username = request.Username,
				Email = request.Email,
				Password = _cryptoService.HashSha256(_cryptoService.DecryptMessage(request.Password)),
				Role = "guest",
				LoginMethod = request.LoginMethod,
				CreatedDate = DateTime.Now,
				LastModified = DateTime.Now
			};

			await _context.Users.AddAsync(user);
			await _context.SaveChangesAsync();
			
			return userToUserDTO(user);
        }

		public async Task<AuthDTO> Login(UserRequest userRequest)
		{
			if (userRequest.LoginMethod == "google")
			{

			}

			User user = await _context.Users
							.FirstOrDefaultAsync(user =>
								(userRequest.Username != null && (userRequest.Username.Equals(user.Username))) ||
								(userRequest.Email != null && userRequest.Email.Equals(user.Email))) ?? throw new NotFoundException();

			if (userRequest.Password != null)
			{
				var password = _cryptoService.HashSha256(_cryptoService.DecryptMessage(userRequest.Password));

				if (password == user.Password)
				{
					user.LastLoggedOn = DateTime.Now;
					await _context.SaveChangesAsync();

                    var token = await generateJwtToken(user);

                    return new AuthDTO(user, token);
				}
				else 
				{
					throw new WrongPasswordException();
				}
			}
			else
			{
				throw new WrongPasswordException();
			}
		}

		public async Task<UserDTO> UpdateUser(UserRequest request)
        {
			User user = await _context.Users
							.FirstOrDefaultAsync(user =>
								(request.Username != null && (request.Username.Equals(user.Username))) ||
								(request.Email != null && request.Email.Equals(user.Email))) ?? throw new NotFoundException();
			
			if (request.Password != null)
			{
				var password = _cryptoService.HashSha256(_cryptoService.DecryptMessage(request.Password));
				string NewPassword = null;

				if (request.NewPassword != null)
				{
					NewPassword = _cryptoService.HashSha256(_cryptoService.DecryptMessage(request.NewPassword));
				}

				if (password == user.Password)
				{
					user.Email = request.Email ?? user.Email;
					user.Password = NewPassword ?? user.Password;
					user.LoginMethod = request.LoginMethod ?? user.LoginMethod;

					await _context.SaveChangesAsync();

					return userToUserDTO(user);
				}
				else 
				{
					throw new WrongPasswordException();
				}
			}
			else
			{
				throw new WrongPasswordException();
			}
        }

		public async Task DeleteUser(UserRequest request)
        {
			User user = await _context.Users
							.FirstOrDefaultAsync(user =>
								(request.Username != null && (request.Username.Equals(user.Username))) ||
								(request.Email != null && request.Email.Equals(user.Email))) ?? throw new NotFoundException();
			
			if (request.Password != null)
			{
				var password = _cryptoService.HashSha256(_cryptoService.DecryptMessage(request.Password));

				if (password == user.Password)
				{
					_context.Remove(user);
					await _context.SaveChangesAsync();
				}
				else 
				{
					throw new WrongPasswordException();
				}
			}
			else
			{
				throw new WrongPasswordException();
			}
        }

        private static UserDTO userToUserDTO(User user)
        {
            return new UserDTO()
            {
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                LoginMethod = user.LoginMethod,
                LastLoggedOn = user.LastLoggedOn
            };
        }

        public async Task<User> FindUserByUsername(string username)
        {
            User user = await _context.Users
				.FirstOrDefaultAsync(user => user.Username == username) ?? throw new NotFoundException();

            return user;
        }

        private async Task<string> generateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = await Task.Run(() =>
            {

                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim("username", user.Username) }),
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                return tokenHandler.CreateToken(tokenDescriptor);
            });

            return tokenHandler.WriteToken(token);
        }
    }
}