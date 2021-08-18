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

namespace XsollaSchoolBackend.Controllers
{
    [Authorize, Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
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
            return Redirect(returnUrl);
        }
    }
}
