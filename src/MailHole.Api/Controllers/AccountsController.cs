using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using MailHole.Api.Auth;
using MailHole.Api.Models.Auth;
using MailHole.Common.Model.Options;
using MailHole.Db.Entities.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MailHole.Api.Controllers
{
    [AuthorizeUsers]
    [Route("api/v1/[controller]")]
    public class AccountsController : Controller
    {

        private readonly SignInManager<MailHoleUser> _signInManager;
        private readonly AuthOptions _authOptions;

        public AccountsController(SignInManager<MailHoleUser> signInManager, AuthOptions authOptions)
        {
            _signInManager = signInManager;
            _authOptions = authOptions;
        }

        [HttpPost]
        [Route("token")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateToken([FromBody] AuthRequest authRequest)
        {
            var user = await _signInManager.UserManager.FindByNameAsync(authRequest.UserName);

            if (user == null) return BadRequest();

            if (!await _signInManager.CanSignInAsync(user) ||
                (_signInManager.UserManager.SupportsUserLockout && await _signInManager.UserManager.IsLockedOutAsync(user))) return BadRequest();

            if (!await _signInManager.UserManager.CheckPasswordAsync(user, authRequest.Password)) return BadRequest();

            if (_signInManager.UserManager.SupportsUserLockout)
            {
                await _signInManager.UserManager.ResetAccessFailedCountAsync(user);
            }
            
            var principal = await _signInManager.CreateUserPrincipalAsync(user);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateJwtSecurityToken(_authOptions.Issuer, _authOptions.Audience, new ClaimsIdentity(principal.Identity), DateTime.Now, DateTime.Now.AddHours(1), DateTime.Now,
                _authOptions.SigningCredentials);

            /* TODO return an explicit model containing the expiration time and maybe a few more informations */
            return Ok(new {Token = handler.WriteToken(token)});
        }
    }
}