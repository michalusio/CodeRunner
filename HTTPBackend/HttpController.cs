using Backend;
using System.Text;
using System.Windows.Forms;

namespace HTTPBackend
{
    [Controller("/code/")]
    public class HttpController : BaseController
    {
        private readonly RichTextBox richTextBoxConsole;
        public HttpController(ILogger logger, RichTextBox richTextBoxConsole) : base(logger)
        {
            this.richTextBoxConsole = richTextBoxConsole;
        }

        [Request(RequestMethodType.PUT, "{userId}")]
        public void CodeInput(ulong userId, [RequestBody] string body)
        {
            var player = PlayerController.Get(userId);

            if (player == null)
            {
                player = PlayerController.Create(userId, new RichTextBoxWriter(richTextBoxConsole));
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

            Response.Close();
        }

        [Request(RequestMethodType.GET, "{userId}")]
        public void CodeGet(ulong userId)
        {
            var player = PlayerController.Get(userId);

            if (player == null)
            {
                Response.StatusCode = 404;
            }
            else
            {
                var codeBuilder = new StringBuilder();
                foreach (var code in player.PlayerCode)
                {
                    codeBuilder.AppendLine(code);
                }
                var bytes = Encoding.UTF8.GetBytes(codeBuilder.ToString());
                Response.OutputStream.Write(bytes, 0, bytes.Length);
                Response.OutputStream.Flush();
                Response.StatusCode = 200;
            }

            Response.Close();
        }
    }
}
