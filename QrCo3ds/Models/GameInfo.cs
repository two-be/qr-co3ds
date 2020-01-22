using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace QrCo3ds.Models
{
    public class GameInfo : AbstractBase
    {
        public string BoxArtFile { get; set; } = string.Empty;
        public string CiaFile { get; set; } = string.Empty;
        public string Developer { get; set; } = string.Empty;
        public string GameplayUrl { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int NumberOfPlayers { get; set; }
        public string Publisher { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }

        public List<CategoryInfo> Categories { get; set; } = new List<CategoryInfo>();
        public List<ScreenshotInfo> Screenshots { get; set; } = new List<ScreenshotInfo>();

        [NotMapped]
        public string BoxArtUrl => $"./games/{Id}/boxArtFile?{Guid.NewGuid().ToString("N")}";
        [NotMapped]
        public string CiaUrl => $"./games/{Id}/CiaFile?{Guid.NewGuid().ToString("N")}";
    }
}
