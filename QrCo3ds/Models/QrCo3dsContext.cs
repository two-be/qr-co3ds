using System;
using Microsoft.EntityFrameworkCore;

namespace QrCo3ds.Models
{
    public class QrCo3dsContext : DbContext
    {
        public DbSet<GameInfo> Games { get; set; }
        public DbSet<GenreInfo> Genres { get; set; }
        public DbSet<ScreenshotInfo> Screenshots { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=QrCo3ds.db");
    }
}
