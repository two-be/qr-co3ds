using System;
namespace QrCo3ds.Models
{
    public class GameInfo : AbstractBase
    {
        public string BoxArtFile { get; set; } = string.Empty;
        public string Developer { get; set; } = string.Empty;
        public string GameplayUrl { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int NumberOfPlayers { get; set; }
        public string Publisher { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
    }
}
