using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webapi.Models.ApiModel;
using Webapi.Models.Data.Service;

namespace Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private AccountService _AccountService;
        public AccountController(AccountService accountService)
        {
            _AccountService = accountService;
        }

        [HttpPost("Signup")]
        public ActionResult Signup(UserRegistrationApiModel model)
        {
            bool IsSuccessfulRegistration = _AccountService.UserAccount(model);
            if (IsSuccessfulRegistration)
            {
                return StatusCode(StatusCodes.Status201Created, new { responseCode = "201", message = "user account sucessfully created!" });
            }

            return BadRequest(new { responseCode = "400", message = "Sorry error occured!" });
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(LoginApiModel model)
        {
            bool IsSuccessfulLoggedIn = _AccountService.UserAccountLogin(model);
            if (IsSuccessfulLoggedIn)
            {
                // request token from IdentityServer 4
                var token = await _AccountService.RequestTokenFromIdentityServerAsync(model.Email);
                return Ok(new { responseCode = "200", accessToken = token.AccessToken,User = token.UserName});

            }
            return BadRequest(new { responseCode = "400", message = "Sorry error occured!" });
        }
    }
}
