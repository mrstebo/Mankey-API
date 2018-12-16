namespace TorrentBackend.Models
{
    public class TorrentObjectModel
    {
        private string _progress;
        public string Hash {get;set;}
        public string Name {get;set;}
        public string Progress
        {
            get => $"{_progress}%"; set
            {
                _progress = value;
            }
        }
    }
}