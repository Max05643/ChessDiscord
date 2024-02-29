using ChessDefinitions;
using ChessGameControllerImplementation;
using Microsoft.Extensions.Configuration;
using SqlServerStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;

namespace SqlServerStorage
{
    /// <summary>
    /// Provides a way to store games in SQL Server DB
    /// </summary>
    public class SqlServerGamesStorage : IGamesStorage
    {

        readonly IChessGameFactory gameFactory;
        readonly string connectionString;

        public SqlServerGamesStorage(IChessGameFactory gameFactory, IConfiguration configuration)
        {
            this.gameFactory = gameFactory;
            connectionString = configuration.GetConnectionString("SqlStorage");

            using var dbContext = new GamesStorageDBContext(connectionString, gameFactory);
            dbContext.Database.EnsureCreated();
        }

        IGamesStorage.IGameHandler? IGamesStorage.AcquireGame(string gameId)
        {
            var dbContext = new GamesStorageDBContext(connectionString, gameFactory);
            var transaction = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);

            var game = dbContext.StoredGames.FromSqlInterpolated($"SELECT * FROM [StoredGames] WITH (XLOCK, ROWLOCK) WHERE Id = {gameId}")
            .FirstOrDefault();

            if (game == null || game.ChessGame == null)
            {
                transaction.Dispose();
                dbContext.Dispose();
                return null;
            }
            else
            {
                return new SqlGameHandler(dbContext, transaction, gameId, game.ChessGame);
            }
        }

        void IGamesStorage.CreateGame(string gameId, IChessGame game)
        {
            using var dbContext = new GamesStorageDBContext(connectionString, gameFactory);
            using var transaction = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);

            var previousGame = dbContext.StoredGames.Find(gameId);

            if (previousGame != null)
            {
                previousGame.ChessGame = game;
                dbContext.StoredGames.Update(previousGame);
            }
            else
            {
                dbContext.StoredGames.Add(new ChessGameDBRecord() { Id = gameId, ChessGame = game });
            }

            dbContext.SaveChanges();

            transaction.Commit();
        }

        void IGamesStorage.RemoveGame(string gameId)
        {
            using var dbContext = new GamesStorageDBContext(connectionString, gameFactory);
            using var transaction = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);

            var game = dbContext.StoredGames.Find(gameId);

            if (game != null)
            {
                dbContext.StoredGames.Remove(game);
            }

            dbContext.SaveChanges();
            transaction.Commit();
        }
    }
}
