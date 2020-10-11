using Backend;
using DBLayer;
using FormFront.DTOs;
using HTTPBackend;
using HTTPBackend.Middlewares;
using System.Linq;
using System.Text;

namespace FormFront.Controllers
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
            using(var context = new CodeRunnerEntities())
            {
                var bytes = Encoding.UTF8.GetBytes(loginData.Password);
                if (context.Users.FirstOrDefault(user => user.Username == loginData.Username && user.PasswordHash == bytes) is Users u)
                {
                    Authentication.SignIn(Response);
                    Authentication.Data[Response].SessionData["PlayerID"] = u.Id;
                }
            }
            
        }

        [Request(RequestMethodType.POST, "logout")]
        public void LogOut()
        {
            Authentication.SignOut(Response);
        }
    }
}
