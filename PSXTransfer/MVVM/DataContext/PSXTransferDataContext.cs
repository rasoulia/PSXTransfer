using Microsoft.EntityFrameworkCore;
using PSXTransfer.WPF.MVVM.Models;
using System.IO;

namespace PSXTransfer.WPF.MVVM.DataContext
{
    public class PSXTransferDataContext : DbContext
    {
        public DbSet<Game> Games { get; set; }
        public PSXTransferDataContext()
        {
            Database.MigrateAsync().Wait();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!Directory.Exists("Database"))
            {
                Directory.CreateDirectory("Database");
            }

            optionsBuilder.UseSqlite(@"Data Source=Database\\PSXTransfer.db");
        }
    }
}
