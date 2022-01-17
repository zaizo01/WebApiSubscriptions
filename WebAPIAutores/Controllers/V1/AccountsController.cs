using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entities;
using WebAPIAutores.Services;

namespace WebAPIAutores.Controllers.V1
{
    [Route("api/v1/Accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<User> signInManager;
        private readonly KeysServices keysServices;

        public AccountsController(UserManager<User> userManager, IConfiguration configuration,
            SignInManager<User> signInManager, KeysServices keysServices)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.keysServices = keysServices;
        }

        [HttpPost("UserRegister", Name = "UserRegister")]
        public async Task<ActionResult<ResponseAuthentication>> UserRegister(UserCredentials userCredentials)
        {
            var user = new User { UserName = userCredentials.Email, Email = userCredentials.Email };
            var result = await userManager.CreateAsync(user, userCredentials.Password);

            if (result.Succeeded)
            {
                await keysServices.CreateKey(user.Id, Entities.KeyType.Free);
                return await BuildToken(userCredentials, user.Id);
            } 
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("UserLogin", Name = "UserLogin")]
        public async Task<ActionResult<ResponseAuthentication>> UserLogin(UserCredentials userCredentials)
        {
            var result = await signInManager.PasswordSignInAsync(userCredentials.Email,
                userCredentials.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await userManager.FindByEmailAsync(userCredentials.Email);
                return await BuildToken(userCredentials, user.Id);
            }
            else
            {
                return BadRequest("Login Failed");
            }
        }
        [HttpGet("RenewToken", Name = "RenewToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ResponseAuthentication>> RenewToken()
        {
            var emailUserClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var userEmail = emailUserClaim.Value;

            var idUserClaim = HttpContext.User.Claims.Where(claim => claim.Type == "id").FirstOrDefault();
            var userId = idUserClaim.Value;

            var userCredentials = new UserCredentials
            {
                Email = userEmail
            };
            return await BuildToken(userCredentials, userId);
        }

        private async Task<ResponseAuthentication> BuildToken(UserCredentials userCredentials, string userId)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", userCredentials.Email),
                new Claim("id", userId)
            };

            var user = await userManager.FindByEmailAsync(userCredentials.Email);
            var claimsDB = await userManager.GetClaimsAsync(user);

            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["keyJwt"])); 
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expirationToken = DateTime.UtcNow.AddYears(1); //AddMinutes(30);
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expirationToken,
                signingCredentials: credentials);

            return new ResponseAuthentication()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expirationToken
            };
        }

        [HttpPost("MakeAdminRol", Name = "MakeAdminRol")]
        public async Task<ActionResult> MakeAdminRol(PutAdminDTO putAdminDTO)
        {
            var user = await userManager.FindByEmailAsync(putAdminDTO.Email);
            await userManager.AddClaimAsync(user, new Claim("IsAdmin", "1"));
            return NoContent();
        }

        [HttpPost("RemoveAdminRol", Name = "RemoveAdminRol")]
        public async Task<ActionResult> RemoveAdminRol(PutAdminDTO putAdminDTO)
        {
            var user = await userManager.FindByEmailAsync(putAdminDTO.Email);
            await userManager.RemoveClaimAsync(user, new Claim("IsAdmin", "1"));
            return NoContent();
        }

     }
}
