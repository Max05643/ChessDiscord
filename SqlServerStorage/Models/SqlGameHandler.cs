using ChessDefinitions;
using ChessGameControllerImplementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlServerStorage.Models
{
    internal class SqlGameHandler : IGamesStorage.IGameHandler
    {
        readonly GamesStorageDBContext dbContext;
        readonly IDbContextTransaction transaction;
        readonly string gameId;
        readonly IChessGame chessGame;

        IChessGame IGamesStorage.IGameHandler.Game => chessGame;

        void IDisposable.Dispose()
        {
            try
            {
                var gameToUpdate = dbContext.StoredGames.Find(gameId);

                if (gameToUpdate != null)
                {
                    gameToUpdate.ChessGame = chessGame;
                    dbContext.Update(gameToUpdate);
                    dbContext.SaveChanges();
                }
                transaction.Commit();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error while saving to the DB", e);
            }
            finally
            {
                transaction.Dispose();
                dbContext.Dispose();
            }
        }

        public SqlGameHandler(GamesStorageDBContext dbContext, IDbContextTransaction transaction, string gameId, IChessGame game)
        {
            this.dbContext = dbContext;
            this.transaction = transaction;
            this.gameId = gameId;
            chessGame = game;
        }
    }
}
