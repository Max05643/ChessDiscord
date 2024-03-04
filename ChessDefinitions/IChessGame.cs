using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDefinitions
{

    /// <summary>
    /// Represents all data about a chess game
    /// </summary>
    public interface IChessGame
    {
        /// <summary>
        /// Information about this game's players
        /// </summary>
        IPlayersDescriptor Players { get; }

        IChessBoard Board { get; }

        /// <summary>
        /// Gets a read-only snapshot of the current game and board state. The snapshot won't reference this object
        /// </summary>
        IChessGameSnapshot GetSnapshot();
    }
}
