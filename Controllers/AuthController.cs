using Microsoft.AspNetCore.Mvc;
using project1.Models.DTO;
using project1.Services;
using project1.Constant;
using project1.Exceptions;

namespace project1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(AuthService authService) : ControllerBase
    {
        private readonly AuthService _authService = authService;

        // GET: api/Auth
        [HttpGet]
        public ActionResult<CryptoDTO> GetPublicKey()
        {
            CryptoDTO cryptoDTO = _authService.GetPublicKey();

            return Ok(ConvertToResponse(ApiConstant.OK, ApiConstant.OK_MESSAGE, cryptoDTO));
        }

        // GET: api/Auth/register
        [HttpPost("register")]
        public async Task<ActionResult<Response<UserDTO>>> RegisterUser(UserRequest request)
        {
            try
            {
                var createdUser = await _authService.RegisterUser(request);
                return Ok(ConvertToResponse(ApiConstant.CREATED, ApiConstant.CREATED_MESSAGE, createdUser));
            }
            catch (UserExistsException)
            {
                return Ok(ConvertToResponse(ApiConstant.OK, ApiConstant.OK_MESSAGE, "USER ALREADY EXISTS"));
            }
            catch (ArgumentException e)
            {
                if (e.Message.Equals("PASSWORD IS EMPTY"))
                {
                    return BadRequest(ConvertToResponse(ApiConstant.BAD_REQUEST, ApiConstant.BAD_REQUEST_MESSAGE, "PASSWORD IS EMPTY"));
                }

                return BadRequest(ConvertToResponse(ApiConstant.BAD_REQUEST, ApiConstant.BAD_REQUEST_MESSAGE, "USERNAME OR EMAIL IS EMPTY"));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: api/Auth/login
        [HttpPost("login")]
        public async Task<ActionResult<Response<UserDTO>>> LoginUser(UserRequest request)
        {
            try
            {
                var user = await _authService.Login(request);
                return Ok(ConvertToResponse(ApiConstant.OK, ApiConstant.OK_MESSAGE, user));
            }
            catch (WrongPasswordException)
            {
                return Unauthorized(ConvertToResponse(ApiConstant.UNAUTHORIZED, ApiConstant.UNAUTHORIZED_MESSAGE, "WRONG PASSWORD"));
            }
            catch (NotFoundException)
            {
                return NotFound(ConvertToResponse(ApiConstant.NOT_FOUND, ApiConstant.NOT_FOUND_MESSAGE, "USER NOT FOUND"));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // PUT: api/Auth/update-user
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update-user")]
        public async Task<ActionResult<Response<UserDTO>>> UpdateUser(UserRequest request)
        {
            try
            {
                var user = await _authService.UpdateUser(request);
                return Ok(ConvertToResponse(ApiConstant.OK, ApiConstant.OK_MESSAGE, user));
            }
            catch (WrongPasswordException)
            {
                return Unauthorized(ConvertToResponse(ApiConstant.UNAUTHORIZED, ApiConstant.UNAUTHORIZED_MESSAGE, "WRONG PASSWORD"));
            }
            catch (NotFoundException)
            {
                return NotFound(ConvertToResponse(ApiConstant.NOT_FOUND, ApiConstant.NOT_FOUND_MESSAGE, "USER NOT FOUND"));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // DELETE: api/Auth/delete-user
        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteProject(UserRequest request)
        {
            try
            {
                await _authService.DeleteUser(request);
                return NoContent();
            }
            catch (WrongPasswordException)
            {
                return Unauthorized(ConvertToResponse(ApiConstant.UNAUTHORIZED, ApiConstant.UNAUTHORIZED_MESSAGE, "WRONG PASSWORD"));
            }
            catch (NotFoundException)
            {
                return NotFound(ConvertToResponse(ApiConstant.NOT_FOUND, ApiConstant.NOT_FOUND_MESSAGE, "USER NOT FOUND"));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static Response<UserDTO> ConvertToResponse(int code, string message, UserDTO userDTO)
        {
            return new Response<UserDTO>() {
                Code = code,
                Message = message,
                Data = userDTO
            };
        }

        private static Response<string> ConvertToResponse(int code, string message, string exceptionMessage)
        {
            return new Response<string>() {
                Code = code,
                Message = message,
                Data = exceptionMessage
            };
        }

        private static Response<CryptoDTO> ConvertToResponse(int code, string message, CryptoDTO cryptoDTO)
        {
            return new Response<CryptoDTO>() {
                Code = code,
                Message = message,
                Data = cryptoDTO
            };
        }
    }
}
