using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace QrCo3ds.Models
{
    public class ScreenshotInfo : AbstractBase
    {
        public string ContentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public int GameId { get; set; }
        public string LocalPath { get; set; } = string.Empty;

        public GameInfo Game { get; set; }
    }
}
