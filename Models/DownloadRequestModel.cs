namespace TorrentBackend.Models
{
    public class DownloadRequestModel
    {
        public string ParentFolder {get;set;}
        public string TorrentType {get;set;}
        public string MagnetUri {get;set;}

    }
}