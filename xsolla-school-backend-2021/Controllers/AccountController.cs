using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using XsollaSchoolBackend.Data;
using XsollaSchoolBackend.Models.Tables;

namespace XsollaSchoolBackend.Controllers
{
    [Authorize, Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepo;

        public AccountController(IAccountRepository accountRepo)
        {
            _accountRepo = accountRepo;
        }

        [HttpGet("google-login")]
        [AllowAnonymous]
        public IActionResult GoogleLogin(string returnUrl)
        {
            string redirectUrl = Url.Action("GoogleResponse", "Account", new { ReturnUrl = returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return new ChallengeResult(GoogleDefaults.AuthenticationScheme, properties);

        }

        [HttpGet("google-response")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        {
            var res = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!res.Succeeded)
                return Unauthorized("Can't create ticket");

            var claims = res.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });


            // Парсим данные от гугла
            User user = new User();
            foreach (var claim in claims)
            {
                if (claim.Type == ClaimTypes.Email)
                    user.Email = claim.Value;
                if (claim.Type == ClaimTypes.NameIdentifier)
                    user.GoogleId = claim.Value;
            }

            var userInfo = _accountRepo.GetUserByEmail(user.Email);
            if (userInfo == null)
                _accountRepo.CreateNewUser(user);

            return Redirect(returnUrl);
        }
    }
}
