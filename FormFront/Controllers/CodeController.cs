using Backend;
using HTTPBackend;
using HTTPBackend.Middlewares;
using System;
using System.Text;
using System.Windows.Forms;

namespace FormFront.Controllers
{
    [Controller("/code/")]
    internal sealed class CodeController : BaseController
    {
        private readonly RichTextBox richTextBoxConsole;
        public CodeController(ILogger logger, RichTextBox richTextBoxConsole) : base(logger)
        {
            this.richTextBoxConsole = richTextBoxConsole;
        }

        [RequiresAuthorization]
        [Request(RequestMethodType.PUT, "")]
        public void CodeInput([RequestBody] string body)
        {
            if (!Authentication.Data[Response].SessionData.TryGetValue("PlayerID", out object playerIdObj) || !(playerIdObj is Guid playerId))
            {
                Response.StatusCode = 403;
                return;
            }

            var player = PlayerController.Get(playerId);

            if (player == null)
            {
                player = PlayerController.Create(playerId, new RichTextBoxWriter(richTextBoxConsole));
                Response.StatusCode = 201;
            }
            else
            {
                Response.StatusCode = 200;
            }

            if (!player.TrySetNewAssembly(body))
            {
                Response.StatusCode = 500;
            }
        }

        [RequiresAuthorization]
        [Request(RequestMethodType.GET)]
        public void CodeGet()
        {
            if (!Authentication.Data[Response].SessionData.TryGetValue("PlayerID", out object playerIdObj) || !(playerIdObj is Guid playerId))
            {
                Response.StatusCode = 403;
                return;
            }

            var player = PlayerController.Get(playerId);

            byte[] bytes;
            if (player == null)
            {
                bytes = Encoding.UTF8.GetBytes("No code uploaded yet!");
                Response.StatusCode = 404;
            }
            else
            {
                var codeBuilder = new StringBuilder();
                foreach (var code in player.PlayerCode)
                {
                    codeBuilder.AppendLine(code);
                }
                bytes = Encoding.UTF8.GetBytes(codeBuilder.ToString());
                Response.StatusCode = 200;
            }

            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.OutputStream.Flush();
        }
    }
}
