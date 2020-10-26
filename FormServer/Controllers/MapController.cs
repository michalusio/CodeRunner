using Backend;
using HTTPBackend;
using HTTPBackend.Middlewares;
using Newtonsoft.Json;
using System;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;
using SystemDll;

namespace FormServer.Controllers
{
    [Controller("/map")]
    internal sealed class MapController : BaseController
    {
        public MapController(ILogger logger) : base(logger)
        {
        }

        [RequiresAuthorization]
        [Request(RequestMethodType.GET, "/{chX},{chY}")]
        public void MapGet(int chX, int chY)
        {
            if (!Authentication.Data[Response].TryGetPlayerId(out _))
            {
                Response.StatusCode = 403;
                return;
            }

            using (var compressionStream = new DeflateStream(Response.OutputStream, CompressionLevel.Optimal))
            {
                var mapData = Singleton<Map>.Instance.GetFlattenedChunk(chX, chY);
                var byteMapData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(mapData));
                Logger.Log($"Streaming {byteMapData.Length} bytes of map data");
                compressionStream.Write(byteMapData, 0, byteMapData.Length);
            }
            Response.StatusCode = 200;
        }
    }
}
