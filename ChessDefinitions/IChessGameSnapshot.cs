using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDefinitions
{
    /// <summary>
    /// Represents a read-only data about a chess game
    /// </summary>
    public interface IChessGameSnapshot
    {
        string Fen { get; }

        GameState State { get; }

        /// <summary>
        /// Information about this game's players
        /// </summary>
        IPlayersDescriptor Players { get; }
    }
}
