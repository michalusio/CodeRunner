using Backend;
using FormFront.DTOs;
using HTTPBackend;
using HTTPBackend.Middlewares;

namespace FormFront
{
    [Controller("/account/")]
    internal sealed class AccountController : BaseController
    {
        public AccountController(ILogger logger) : base(logger)
        {
        }

        [Request(RequestMethodType.POST, "login")]
        public void LogIn([RequestBody] LoginDTO loginData)
        {
            if (loginData.Username == "admin" && loginData.Password == "admin")
            {
                Authentication.SignIn(Response);
            }
        }

        [Request(RequestMethodType.POST, "logout")]
        public void LogOut()
        {
            Authentication.SignOut(Response);
        }
    }
}
