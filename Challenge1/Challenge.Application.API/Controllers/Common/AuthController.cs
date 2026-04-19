using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using challenge1.Application.Bll.Auth;
using challenge1.Application.Bll.Auth.Interfaces;
using challenge1.Application.DTO.Auth;
using challenge1.Common.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace challenge1.Application.API.Controllers.Common
{
    [ApiController]
    [Route("Common/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogging _logging;
        private readonly IAuthBll _authBll;
        private readonly IConfiguration _configuration;

        public AuthController(ILogging logging, IAuthBll authBll, IConfiguration configuration)
        {
            _logging = logging;
            _authBll = authBll;
            _configuration = configuration;
        }

        /// <summary>
        /// Authenticate with loginId and password. Returns a Bearer JWT valid for 8 hours.
        /// Default dev credentials: loginId=admin, password=Admin@1234
        /// </summary>
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDTO request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.LoginId) || string.IsNullOrWhiteSpace(request.Password))
                    return BadRequest("LoginId and Password are required.");

                var user = await _authBll.ValidateCredentialsAsync(request.LoginId, request.Password);
                if (user == null)
                    return Unauthorized("Invalid credentials.");

                var secret = _configuration["Jwt:Secret"]!;
                var expiresAt = DateTime.UtcNow.AddHours(8);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.LoginId),
                    new Claim(JwtRegisteredClaimNames.Name, user.FullName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: expiresAt,
                    signingCredentials: creds);

                return Ok(new LoginResponseDTO
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    LoginId = user.LoginId,
                    FullName = user.FullName,
                    ExpiresAt = expiresAt
                });
            }
            catch (Exception ex)
            {
                _logging.LogControllerError(ex.ToString());
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
