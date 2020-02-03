using System;
using Microsoft.EntityFrameworkCore;

namespace QrCo3ds.Models
{
    public class QrCo3dsContext : DbContext
    {
        public DbSet<CategoryInfo> Categories { get; set; }
        public DbSet<DlcInfo> Dlcs { get; set; }
        public DbSet<GameInfo> Games { get; set; }
        public DbSet<ScreenshotInfo> Screenshots { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=../Database/QrCo3ds.db");
    }
}
