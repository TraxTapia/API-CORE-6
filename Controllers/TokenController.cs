using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Web.Api.DAO.ContextBD;
using Web.Api.Models.Models;
using Web.Api.Models.Models.Identity;
using Web.Api.Models.OperationResult;

namespace WEBAPITRAX.Controllers
{
    [Route("api/Token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly DataBaseContext _context;

        public TokenController(IConfiguration config, DataBaseContext context)
        {
            _configuration = config;
            _context = context;
        }

        //[HttpPost]
        //public async Task<IActionResult> Post(UserInfo _userData)
        //{
        //    if (_userData != null && _userData.Email != null && _userData.Password != null)
        //    {
        //        var user = await GetUser(_userData.Email, _userData.Password);

        //        if (user != null)
        //        {
        //            //create claims details based on the user information
        //            var claims = new[] {
        //                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
        //                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
        //                new Claim("UserId", user.UserId.ToString()),
        //                new Claim("DisplayName", user.DisplayName),
        //                new Claim("UserName", user.UserName),
        //                new Claim("Email", user.Email)
        //            };

        //            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        //            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //            var token = new JwtSecurityToken(
        //                _configuration["Jwt:Issuer"],
        //                _configuration["Jwt:Audience"],
        //                claims,
        //                expires: DateTime.UtcNow.AddMinutes(10),
        //                signingCredentials: signIn);

        //            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        //        }
        //        else
        //        {
        //            return BadRequest("Invalid credentials");
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest();
        //    }
        //}
        [HttpPost]
        public async Task<LoginResponseDTO> GetToken(UserInfo _userData)
        {
            LoginResponseDTO _Response = new LoginResponseDTO();

            if (_userData != null && _userData.Email != null && _userData.Password != null)
            {

                var user = await GetUser(_userData.Email, _userData.Password);

                if (user != null)
                {
                    //create claims details based on the user information
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", user.UserId.ToString()),
                        new Claim("DisplayName", user.DisplayName),
                        new Claim("UserName", user.UserName),
                        new Claim("Email", user.Email)
                    };
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(10),
                        signingCredentials: signIn);

                    _Response.AccessToken = new JwtSecurityTokenHandler().WriteToken(token);
                    _Response.TokenType = "bearer";
                    _Response.ExpiresIn = token.ValidTo;
                    _Response.Username = user.UserName;


                    //Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    throw new Exception("Credenciales Invalidas");
                }
            }
            else
            {
                throw new Exception("Los parametros no pueden estar vacios");
            }
            return _Response;

        }

        private async Task<UserInfo> GetUser(string email, string password)
        {
            if (true)
            {

            }
            return await _context.UserInfos.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }
    }
}
