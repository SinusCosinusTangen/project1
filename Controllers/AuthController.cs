using Microsoft.AspNetCore.Mvc;
using project1.Models.DTO;
using project1.Services;
using project1.Constant;
using project1.Exceptions;
using project1.Helpers;

namespace project1.Controllers
{
    // [Route("api/[controller]")]
    // [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        // GET: api/Auth
        // [HttpGet]
        public ActionResult<Response<CryptoDTO>> GetPublicKey()
        {
            CryptoDTO cryptoDTO = _authService.GetPublicKey();

            return Ok(new Response<CryptoDTO>(ApiConstant.OK, ApiConstant.OK_MESSAGE, cryptoDTO));
        }

        // [HttpGet("encrypt/{request}")]
        public ActionResult<Response<CryptoDTO>> Encrypt(string request)
        {
            CryptoDTO cryptoDTO = _authService.GetEncryptedPassword(request);

            return Ok(new Response<CryptoDTO>(ApiConstant.OK, ApiConstant.OK_MESSAGE, cryptoDTO));
        }

        // GET: api/Auth/register
        // [HttpPost("register")]
        public async Task<ActionResult<Response<UserDTO>>> RegisterUser(UserRequest request)
        {
            try
            {
                var createdUser = await _authService.RegisterUser(request);
                return Ok(new Response<UserDTO>(ApiConstant.CREATED, ApiConstant.CREATED_MESSAGE, createdUser));
            }
            catch (UserExistsException)
            {
                return Ok(new Response<string>(ApiConstant.OK, ApiConstant.OK_MESSAGE, "USER ALREADY EXISTS"));
            }
            catch (ArgumentException e)
            {
                if (e.Message.Equals("PASSWORD IS EMPTY"))
                {
                    return BadRequest(new Response<string>(ApiConstant.BAD_REQUEST, ApiConstant.BAD_REQUEST_MESSAGE, "PASSWORD IS EMPTY"));
                }

                return BadRequest(new Response<string>(ApiConstant.BAD_REQUEST, ApiConstant.BAD_REQUEST_MESSAGE, "USERNAME OR EMAIL IS EMPTY"));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: api/Auth/login
        // [HttpPost("login")]
        public async Task<ActionResult<Response<AuthDTO>>> LoginUser(UserRequest request)
        {
            try
            {
                var user = await _authService.Login(request);
                return Ok(new Response<AuthDTO>(ApiConstant.OK, ApiConstant.OK_MESSAGE, user));
            }
            catch (WrongPasswordException)
            {
                return Unauthorized(new Response<string>(ApiConstant.UNAUTHORIZED, ApiConstant.UNAUTHORIZED_MESSAGE, "WRONG PASSWORD"));
            }
            catch (NotFoundException)
            {
                return NotFound(new Response<string>(ApiConstant.NOT_FOUND, ApiConstant.NOT_FOUND_MESSAGE, "USER NOT FOUND"));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // PUT: api/Auth/update-user
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPut("update-user")]
        // [Authorize]
        public async Task<ActionResult<Response<UserDTO>>> UpdateUser(UserRequest request)
        {
            try
            {
                var user = await _authService.UpdateUser(request);
                return Ok(new Response<UserDTO>(ApiConstant.OK, ApiConstant.OK_MESSAGE, user));
            }
            catch (WrongPasswordException)
            {
                return Unauthorized(new Response<string>(ApiConstant.UNAUTHORIZED, ApiConstant.UNAUTHORIZED_MESSAGE, "WRONG PASSWORD"));
            }
            catch (NotFoundException)
            {
                return NotFound(new Response<string>(ApiConstant.NOT_FOUND, ApiConstant.NOT_FOUND_MESSAGE, "USER NOT FOUND"));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // DELETE: api/Auth/delete-user
        // [HttpDelete("delete-user")]
        // [Authorize]
        public async Task<IActionResult> DeleteProject(UserRequest request)
        {
            try
            {
                await _authService.DeleteUser(request);
                return NoContent();
            }
            catch (WrongPasswordException)
            {
                return Unauthorized(new Response<string>(ApiConstant.UNAUTHORIZED, ApiConstant.UNAUTHORIZED_MESSAGE, "WRONG PASSWORD"));
            }
            catch (NotFoundException)
            {
                return NotFound(new Response<string>(ApiConstant.NOT_FOUND, ApiConstant.NOT_FOUND_MESSAGE, "USER NOT FOUND"));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
