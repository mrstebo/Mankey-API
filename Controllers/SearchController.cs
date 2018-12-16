using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ArcaneLib;
using System.IO;
using System.Net;

namespace TorrentBackend.Controllers
{
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            return Content("GET Requests Are Not Allowed.");
        }
        
        [HttpPost("/api/Search/{query}")]
        public async Task<JsonResult> Get(string query)
        {
            NetworkCredential networkCred = new NetworkCredential((string)Program.config.proxy.username, (string)Program.config.proxy.password);
            IWebProxy proxy = new WebProxy($"{Program.config.proxy.uri}:{Program.config.proxy.port}",false,null,networkCred);
            using(ArcaneLib.TorrentSearch Search = new TorrentSearch((string)Program.config.endpoint,proxy))
            {
                try
                {
                    return Json(await Search.Search(query));
                }
                catch(Exception e)
                {
                    return Json(e);
                }
            }
        }

    }
}