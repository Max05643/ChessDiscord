using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ChessDefinitions.IChessGame;

namespace ChessDefinitions
{
    /// <summary>
    /// Represents a state of the chess board
    /// </summary>
    public interface IChessBoard
    {
        bool IsWhiteMove();
        GameState GetCurrentState();
        string GetFen();

        /// <summary>
        /// Makes a move if it is possible
        /// </summary>
        /// <param name="move">Move in algebraic notation, for example e2e4</param>
        /// <returns>Whether move was possible and was made</returns>
        bool MakeMove(string move);
    }
}
