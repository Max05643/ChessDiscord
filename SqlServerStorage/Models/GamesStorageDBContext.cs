using ChessDefinitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlServerStorage.Models
{
    internal class GamesStorageDBContext : DbContext
    {

        readonly string connectionString;
        readonly IChessGameFactory gameFactory;

        public GamesStorageDBContext(string connectionString, IChessGameFactory gameFactory)
        {
            this.connectionString = connectionString;
            this.gameFactory = gameFactory;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var converter = new ValueConverter<IChessGame?, string?>(
                v => Conversions.IChessGameToString(v),
                v => Conversions.StringToIChessGame(v, gameFactory));

            modelBuilder.Entity<ChessGameDBRecord>()
                .Property(e => e.ChessGame)
                .HasConversion(converter);
        }

        public DbSet<ChessGameDBRecord> StoredGames => Set<ChessGameDBRecord>();
    }
}
