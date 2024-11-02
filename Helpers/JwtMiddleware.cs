using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using project1.Exceptions;
using project1.Models;
using project1.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace project1.Helpers
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings, IConnectionMultiplexer connectionMultiplexer)
        {
            _next = next;
            _appSettings = appSettings.Value;
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // If there's no token, just proceed
            if (string.IsNullOrEmpty(token))
            {
                await _next(context);
                return;
            }

            // Validate the token and extract the username
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            string usernameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "username")?.Value + ":ADMINISTRATOR";

            if (string.IsNullOrEmpty(usernameClaim))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized; // Unauthorized
                await context.Response.WriteAsync("Invalid token: No username claim found.");
                return;
            }

            // Get the database from the connection multiplexer
            var db = _connectionMultiplexer.GetDatabase();

            // Check if the token exists in Redis using the username as the key
            var redisToken = await db.ListRangeAsync(usernameClaim);
            var tokenList = redisToken.Select(t => t.ToString()).ToList();

            // Ensure redisToken is not null or empty
            if (tokenList.Count == 0 || !tokenList.Contains(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized; // Unauthorized
                await context.Response.WriteAsync("Token is invalid or revoked.");
                return;
            }

            // Token is valid; attach user to context and proceed
            // context.Items["User"] = usernameClaim;
            await AttachUserToContext(context, usernameClaim, token);
            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, string username, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

                // Validate the token and extract claims
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                // Attach user to HttpContext
                context.Items["User"] = username;
            }
            catch (NotFoundException)
            {
                Console.WriteLine("User not found");
            }
            catch (SecurityTokenExpiredException)
            {
                Console.WriteLine("Token has expired");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized; // Unauthorized
                await context.Response.WriteAsync("Token has expired.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
                // Optionally handle or log unexpected exceptions
                // Do nothing if JWT validation fails
                // User is not attached to context so the request won't have access to secure routes
                throw ex;
            }
        }
    }
}
