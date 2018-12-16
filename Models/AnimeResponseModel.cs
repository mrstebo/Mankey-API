namespace mankey_api.Models
{
    public class AnimeResponseModel
    {
        public string type {get => "Anime";}
        public string name {get;set;}
        public string uploadDate {get;set;}
        public string size {get => "N/A";}
        public string seeders {get => "N/A";}
        public string leechers {get => "N/A";}
        public string uploaderType {get => "HorribleSubs";}
        public string uri {get => "https://horriblesubs.info";}
        public string magnet {get;set;}
        public string parent {get;set;}

    }
}