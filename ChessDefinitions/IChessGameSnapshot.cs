using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDefinitions
{
    /// <summary>
    /// Represents a read-only state of a chess game
    /// </summary>
    public interface IChessGameSnapshot
    {
        public string Fen { get; }

        public IChessGame.GameState State { get; }

        /// <summary>
        /// Is player playing as white and AI as black?
        /// </summary>
        bool IsPlayerWhite { get; }
    }
}
