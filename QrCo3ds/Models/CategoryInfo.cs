using System;
namespace QrCo3ds.Models
{
    public class CategoryInfo : AbstractBase
    {
        public int GameId { get; set; }
        public string Name { get; set; } = string.Empty;

        public GameInfo Game { get; set; }
    }
}
