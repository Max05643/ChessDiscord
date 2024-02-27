using ChessDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGameControllerImplementation
{
    /// <summary>
    /// Stores games in a thread-safe way in memory
    /// </summary>
    public class InMemoryGamesStorage : IGamesStorage, IDisposable
    {
        readonly Dictionary<string, IChessGame> games = new();
        readonly Mutex mutex = new(false);

        class GameHandler : IGamesStorage.IGameHandler
        {
            readonly Mutex mutex;
            readonly IChessGame game;

            public GameHandler(Mutex mutex, IChessGame game)
            {
                this.mutex = mutex;
                this.game = game;
            }

            IChessGame IGamesStorage.IGameHandler.Game => game;

            void IDisposable.Dispose()
            {
                mutex.ReleaseMutex();
            }
        }

        IGamesStorage.IGameHandler? IGamesStorage.AcquireGame(string gameId)
        {
            mutex.WaitOne();

            if (games.ContainsKey(gameId))
                return new GameHandler(mutex, games[gameId]);
            else
            {
                mutex.ReleaseMutex();
                return null;
            }

        }

        void IGamesStorage.CreateGame(string gameId, IChessGame game)
        {
            mutex.WaitOne();

            games[gameId] = game;

            mutex.ReleaseMutex();
        }

        void IGamesStorage.RemoveGame(string gameId)
        {
            mutex.WaitOne();

            if (games.ContainsKey(gameId))
                games.Remove(gameId);

            mutex.ReleaseMutex();
        }

        void IDisposable.Dispose()
        {
            mutex.Dispose();
        }
    }
}
