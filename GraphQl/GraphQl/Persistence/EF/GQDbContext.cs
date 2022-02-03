using GraphQl.Models;
using Microsoft.EntityFrameworkCore;

namespace GraphQl
{
    public class GQDbContext : DbContext
    {
        public GQDbContext(DbContextOptions<GQDbContext> options) : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.LogTo(Console.WriteLine);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FarmableAcresRecord>().HasNoKey();

            modelBuilder.Entity<Farm>().HasMany<FarmableAcresRecord>();
        }

        public DbSet<User> Users { get; set; }
    }
}
