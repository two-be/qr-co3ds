using System;
namespace QrCo3ds.Models
{
    public class GenreInfo : AbstractBase
    {
        public int GameId { get; set; }

        public GameInfo Game { get; set; }
    }
}
