using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDefinitions
{
    /// <summary>
    /// Represents a service that generates best chess moves depending on current position
    /// </summary>
    public interface IChessAI
    {
        /// <summary>
        /// Returns next best move if it is possible
        /// </summary>
        /// <param name="currentPositionFen">Current position in FEN</param>
        /// <param name="move">Next move. Will be null if impossible to calculate</param>
        /// <param name="depth">Max search depth of the chess AI</param>
        bool GetNextMove(string currentPositionFen, out string? move, int depth = 15);
    }
}
