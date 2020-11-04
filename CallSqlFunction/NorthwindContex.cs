using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CallSqlFunction
{
    public class NorthwindContex : DbContext
    {
        private readonly string _connectionString;

        public NorthwindContex(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<SearchResult> SearchResults { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SearchResult>().HasNoKey();
            modelBuilder.Entity<SearchResult>().Property(x => x.Id).HasColumnName("p_id");
            modelBuilder.Entity<SearchResult>().Property(x => x.Name).HasColumnName("p_name");
        }
    }
}
