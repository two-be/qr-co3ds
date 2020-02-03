using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace QrCo3ds.Models
{
    public class GameInfo : AbstractBase
    {
        public string BoxArtLocalPath { get; set; } = string.Empty;
        public string CiaLocalPath { get; set; } = string.Empty;
        public string Developer { get; set; } = string.Empty;
        public string GameplayUrl { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int NumberOfPlayers { get; set; }
        public string Publisher { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }

        public List<CategoryInfo> Categories { get; set; } = new List<CategoryInfo>();
        public List<DlcInfo> Dlcs { get; set; } = new List<DlcInfo>();
        public List<ScreenshotInfo> Screenshots { get; set; } = new List<ScreenshotInfo>();

        [NotMapped]
        public IFormFile BoxArtFile { get; set; }
        [NotMapped]
        public string BoxArtUrl => $"./games/{Id}/boxArtFile?{Guid.NewGuid().ToString("N")}";
        [NotMapped]
        public IFormFile CiaFile { get; set; }
        [NotMapped]
        public string CiaUrl => $"./games/{Id}/CiaFile?{Guid.NewGuid().ToString("N")}";
    }
}
