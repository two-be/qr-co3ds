using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QrCo3ds.Models
{
    public class DlcInfo : AbstractBase
    {
        public string ContentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public int GameId { get; set; }
        public string LocalPath { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public GameInfo Game { get; set; }

        [NotMapped]
        public string FileUrl => $"./dlcs/{Id}/file?{Guid.NewGuid().ToString("N")}";
    }
}
