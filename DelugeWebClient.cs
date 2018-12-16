using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TorrentBackend.Models;

namespace TorrentBackend
{
    public class DelugeWebClient : WebClient, IDisposable
    {
        public CookieContainer Cookies { get; internal set; }
        private const string address = "http://127.0.0.1:8112/json";

        public DelugeWebClient(string delugeWebPassword)
        {
            #if DEBUG
            Console.WriteLine("Password passed in: " + delugeWebPassword);
            #endif
            CreateSession(delugeWebPassword);
        }

		protected override WebRequest GetWebRequest (Uri address)
        {
			var request = base.GetWebRequest (address);
            request.ContentType = "application/json";
            request.Timeout = 5000;
			if (request is HttpWebRequest)
            {
				if (Cookies == null) 
                {
                    Cookies = new CookieContainer ();
                }
				(request as HttpWebRequest).CookieContainer = Cookies;
                (request as HttpWebRequest).AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
			}
			return request;
        }

        private void CreateSession(string delugeWebPassword)
        {
            string payload = "{\"method\":\"auth.login\",\"params\":[\"" + delugeWebPassword + "\"],\"id\":16}";
            var bytes = System.Text.Encoding.UTF8.GetBytes(payload);
            this.UploadData(address,"POST",bytes);
        }

        public async Task AddTorrent(string payload)
        {
            await this.UploadStringTaskAsync(address, "POST",payload);
        }

        public async Task DeleteTorrent(string hash)
        {
            var payload = $"{{\"method\":\"core.remove_torrent\",\"params\":[\"{hash}\",true],\"id\":67}}";
            try
            {
                await this.UploadStringTaskAsync(address, "POST", payload);
            }
            catch(WebException)
            {
                //timed out most likely.
            }
        }

        public async Task<List<TorrentObjectModel>> GetCurrentTorrents()
        {
            const string payload = @"{""method"":""web.update_ui"",""params"":[[""queue"",""name"",""total_wanted"",""state"",""progress"",""num_seeds"",""total_seeds"",""num_peers"",""total_peers"",""download_payload_rate"",""upload_payload_rate"",""eta"",""ratio"",""distributed_copies"",""is_auto_managed"",""time_added"",""tracker_host"",""save_path"",""total_done"",""total_uploaded"",""max_download_speed"",""max_upload_speed"",""seeds_peers_ratio""],{}],""id"":506}";
            var bytes = System.Text.Encoding.UTF8.GetBytes(payload);
            var respBytes = await this.UploadDataTaskAsync(address,"POST",bytes);
            var json = Newtonsoft.Json.Linq.JObject.Parse(System.Text.Encoding.UTF8.GetString(respBytes));
            List<TorrentObjectModel> values = new List<TorrentObjectModel>();
            foreach(Newtonsoft.Json.Linq.JToken tok in json.SelectTokens("result.torrents.*"))
            {
                if(tok.Value<string>("total_wanted") == "0")
                {
                    continue; //ignore the proxy tracker
                }
                var hash = tok.Path.Substring(tok.Path.LastIndexOf('.') + 1);
                var name = tok.Value<string>("name");
                Decimal.TryParse(tok.Value<string>("progress"),out var prog);
                var progress = prog.ToString ("0.##");
                values.Add(new TorrentObjectModel() {Hash = hash, Name = name, Progress = progress});
            }
            return values;
        }

    }
}