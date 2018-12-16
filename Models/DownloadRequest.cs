namespace TorrentBackend.Models
{
    public class DownloadRequest
    {
        public string ParentFolder {get;set;}
        public string TorrentType {get;set;}
        public string MagnetUri {get;set;}

    }
}