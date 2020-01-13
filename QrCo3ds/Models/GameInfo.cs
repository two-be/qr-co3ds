using System;
namespace QrCo3ds.Models
{
    public class GameInfo : AbstractBase
    {
        public string CoverFile { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string GameplayUrl { get; set; } = string.Empty;
    }
}
