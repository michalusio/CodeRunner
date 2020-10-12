using Backend;
using DBLayer;
using FormServer.DTOs;
using HTTPBackend;
using HTTPBackend.Middlewares;
using System.Linq;
using System.Text;

namespace FormServer.Controllers
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
            if (DBMiddleware.Context.Users.FirstOrDefault(u => u.Email == loginData.Email) is User user)
            {
                if (BCrypt.Net.BCrypt.Verify(loginData.Password, Encoding.ASCII.GetString(user.PasswordHash)))
                {
                    Authentication.SignIn(Response);
                    Authentication.Data[Response].SessionData["PlayerID"] = user.Id;
                    Response.StatusCode = 200;
                    return;
                }
            }
            Response.StatusCode = 401;
        }

        [RequiresAuthorization]
        [Request(RequestMethodType.POST, "logout")]
        public void LogOut()
        {
            Authentication.SignOut(Response);
        }

        [Request(RequestMethodType.POST, "register")]
        public void Register([RequestBody] RegisterDTO registerData)
        {
            if (DBMiddleware.Context.Users.FirstOrDefault(u => u.Email == registerData.Email || u.Username == registerData.Username) is User user)
            {
                Response.StatusCode = 409;
                Response.StatusDescription = "A user with this e-mail/username already exists!";
                return;
            }

            user = new User
            {
                Username = registerData.Username,
                Email = registerData.Email,
                PasswordHash = Encoding.ASCII.GetBytes(BCrypt.Net.BCrypt.HashPassword(registerData.Password, 12))
            };

            DBMiddleware.Context.Users.Add(user);
            DBMiddleware.Context.SaveChanges();
            Response.StatusCode = 201;
        }
    }
}
