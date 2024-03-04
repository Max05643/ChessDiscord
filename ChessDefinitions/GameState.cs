using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDefinitions
{
    /// <summary>
    /// Represents a state of a chess game 
    /// </summary>
    public enum GameState
    {
        InProgress = 0,
        Stalemate = 1,
        BlackWon = 2,
        WhiteWon = 3
    }

}
