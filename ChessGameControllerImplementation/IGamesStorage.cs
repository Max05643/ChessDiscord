using ChessDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGameControllerImplementation
{
    /// <summary>
    /// Represents a way to store chess games in a thread-safe manner
    /// </summary>
    public interface IGamesStorage
    {
        /// <summary>
        /// Provides a way to get the exclusive access to the game and the ability to terminate access.
        /// Releases the game when disposed
        /// </summary>
        public interface IGameHandler : IDisposable
        {
            public IChessGame Game { get; }
        }

        /// <summary>
        /// Provides an exclusive access to the game with specified id. Returns null if game does not exist
        /// </summary>
        IGameHandler? AcquireGame(string gameId);

        /// <summary>
        /// Removes a game with specified id. Does nothing if game does not exist
        /// </summary>
        void RemoveGame(string gameId);

        /// <summary>
        /// Creates a new game with specified gameId. Overwrites old one if it exists
        /// </summary>
        void CreateGame(string gameId, IChessGame game);
    }
}
