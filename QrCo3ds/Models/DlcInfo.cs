using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QrCo3ds.Models
{
    public class DlcInfo : AbstractBase
    {
        public int GameId { get; set; }
        public string LocalPath { get; set; }
        public string Name { get; set; } = string.Empty;

        public GameInfo Game { get; set; }
    }
}
