using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDefinitions
{

    /// <summary>
    /// Represents a chess game
    /// </summary>
    public interface IChessGame
    {
        /// <summary>
        /// Represents a state of a chess game 
        /// </summary>
        enum GameState
        {
            InProgress = 0,
            Stalemate = 1,
            BlackWon = 2,
            WhiteWon = 3
        }

        string GetFen();
        void LoadFromFen(string positionFen);

        /// <summary>
        /// Makes a move if it is possible
        /// </summary>
        /// <param name="move">Move in algebraic notation, for example e2e4</param>
        /// <returns>Whether move was possible and was made</returns>
        bool MakeMove(string move);

        /// <summary>
        /// Is player playing as white and AI as black?
        /// </summary>
        bool IsPlayerWhite { get; }

        bool IsWhiteMove();
        GameState GetCurrentState();

        string ToAscii();

        /// <summary>
        /// Provides a clone of current game state 
        /// </summary>
        IChessGame Clone();
    }
}
