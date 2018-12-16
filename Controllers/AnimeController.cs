using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using mankey_api.Models;
using HtmlAgilityPack;

namespace TorrentBackend.Controllers
{
    [Route("api/[controller]")]
    public class AnimeController : Controller
    {
        const string horribleSubsUrl = "https://horriblesubs.info/";

        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            return Content("GET Requests Are Not Allowed.");
        }

        [HttpPost("/api/anime/all/")]
        public async Task<JsonResult> Post()
        {
            string dirtyRegex = @"<div class=""ind-show""><a[^>]* href=""([^""]*)""[ ]?title=""([^""]*)"">"; // it's 2:32am leave me and this code alone!
            List<Dictionary<string,string>> values = new List<Dictionary<string, string>>();
            using(WebClient WC = new WebClient())
            {
                string response = await WC.DownloadStringTaskAsync(horribleSubsUrl + "shows/");
                var regexMatches = Regex.Matches(response, dirtyRegex);
                foreach(Match match in regexMatches)
                {
                    var tmpdictionary = new Dictionary<string,string>();
                    tmpdictionary.Add("name",match.Groups[2].Value);
                    tmpdictionary.Add("url",match.Groups[1].Value);
                    values.Add(tmpdictionary);
                }
            }
            return Json(values);
        }

        [HttpPost("/api/anime/episodes/shows/{url}")]
        public async Task<JsonResult> Episodes(string url)
        {
            url = "shows/" + url;
            List<AnimeResponse> values = new List<AnimeResponse>();
            using(WebClient WC = new WebClient())
            {
                var firstResponse = await WC.DownloadStringTaskAsync(horribleSubsUrl + url);
                //need to grab show ID!
                var match = Regex.Match(firstResponse,"var hs_showid[ ]?=[ ]?([0-9]+)");
                int.TryParse(match.Groups[1].Value, out var showid);
                var secondResponse = await WC.DownloadStringTaskAsync(horribleSubsUrl + "api.php?method=getshows&type=show&showid=" + showid);
                //manipulate response.
                var agilityPack = new HtmlDocument();
                agilityPack.LoadHtml(secondResponse);
                foreach(var row in agilityPack.DocumentNode.SelectNodes("//*[contains(@class,'rls-info-container')]"))
                {
                    var newobject = new AnimeResponse();
                    var mainrow = row.ChildNodes.FirstOrDefault(x => x.HasClass("rls-label"));
                    var date = mainrow.ChildNodes.FirstOrDefault(x => x.HasClass("rls-date")).InnerText.Trim();
                    var name = mainrow.ChildNodes.FirstOrDefault(x => x.NodeType == HtmlNodeType.Text && x.InnerText.Trim() != "").InnerText.Trim();
                    var episode = mainrow.SelectNodes(row.XPath + "//strong[1]").FirstOrDefault().InnerText.Trim();
                    var magneturi = row.SelectSingleNode(row.XPath + "//*[contains(@class,'link-1080p')]//*[contains(@class,'hs-magnet-link')]//a[1]").GetAttributeValue("href","");
                    newobject.name = name + " - " + episode;
                    newobject.parent = name;
                    newobject.uploadDate = date;
                    newobject.magnet = magneturi;
                    values.Add(newobject);
                }
                return Json(values);
            }
        }
    }
}