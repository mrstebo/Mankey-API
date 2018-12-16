using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net;

namespace TorrentBackend.Controllers
{
    [Route("api/[controller]")]
    public class TorrentController : Controller
    {
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            return Content("GET Requests Are Not Allowed.");
        }

        [HttpPost("/api/Download/Current/")]
        public async Task<IActionResult> Current()
        {
            DelugeWebClient delugeWebClient = new DelugeWebClient(Program.config.deluge_web_password);
            var responseObject = await delugeWebClient.GetCurrentTorrents();
            delugeWebClient.Dispose();
            return Json(responseObject);
        }

        [HttpDelete("/api/Download/{hash}")]
        public async Task<IActionResult> Delete(string hash)
        {
            DelugeWebClient delugeWebClient = new DelugeWebClient(Program.config.deluge_web_password);
            await delugeWebClient.DeleteTorrent(hash);
            delugeWebClient.Dispose();
            return Content("");
        }

        [HttpPost("/api/Download/")]
        public async Task<IActionResult> Get([FromBody]Models.DownloadRequestModel downloadRequest)
        {
            string type = downloadRequest.TorrentType.ToLower();
            string mappedType = "";
            //Map all possible TPB main media types
            if(type.Contains("porn"))
            {
                mappedType = "18/Videos/";
            }
            else if(type.Contains("tv"))
            {
                mappedType = "TV/";
            }
            else if(type.Contains("movie"))
            {
                mappedType = "Films/";
            }
            else if(type.Contains("anime"))
            {
                mappedType = "Anime/";
            }
            else
            {
                mappedType = "Other/";
            }
            if(downloadRequest.ParentFolder != "")
            {
                mappedType += downloadRequest.ParentFolder + "/";
            }
            string payload = System.IO.File.ReadAllText("example.json").Replace("{MAGNETURI}",downloadRequest.MagnetUri).Replace("{TYPEMAPPED}",mappedType);
            try
            {
                DelugeWebClient delugeWebClient = new DelugeWebClient((string)Program.config.deluge_web_password);
                await delugeWebClient.AddTorrent(payload);
                delugeWebClient.Dispose();
                return Content("");
            }
            catch(Exception e)
            {
                return Content(e.ToString());
            }

        }
    }
}