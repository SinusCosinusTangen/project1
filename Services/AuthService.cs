using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using project1.Exceptions;
using project1.Models;
using project1.Models.DTO;

namespace project1.Services
{
	public class AuthService(UserContext userContext, CryptoService cryptoService)
    {
		private readonly UserContext _context = userContext;
		private readonly CryptoService _cryptoService = cryptoService;

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
				Password = CryptoService.HashSha256(CryptoService.DecryptMessage(request.Password)),
				Role = "guest",
				LoginMethod = request.LoginMethod,
				CreatedDate = DateTime.Now,
				LastModified = DateTime.Now
			};

			await _context.Users.AddAsync(user);
			await _context.SaveChangesAsync();
			
			return UserToUserDTO(user);
        }

		public async Task<UserDTO> Login(UserRequest userRequest)
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
				var password = CryptoService.HashSha256(CryptoService.DecryptMessage(userRequest.Password));

				if (password == user.Password)
				{
					user.LastLoggedOn = DateTime.Now;
					await _context.SaveChangesAsync();

					return UserToUserDTO(user);
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
				var password = CryptoService.HashSha256(CryptoService.DecryptMessage(request.Password));
				string NewPassword = null;

				if (request.NewPassword != null)
				{
					NewPassword = CryptoService.HashSha256(CryptoService.DecryptMessage(request.NewPassword));
				}

				if (password == user.Password)
				{
					user.Email = request.Email ?? user.Email;
					user.Password = NewPassword ?? user.Password;
					user.LoginMethod = request.LoginMethod ?? user.LoginMethod;

					await _context.SaveChangesAsync();

					return UserToUserDTO(user);
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
				var password = CryptoService.HashSha256(CryptoService.DecryptMessage(request.Password));

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

        private static UserDTO UserToUserDTO(User user)
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
    }
}