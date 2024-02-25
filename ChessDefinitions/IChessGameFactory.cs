using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDefinitions
{
    /// <summary>
    /// Creates new games
    /// </summary>
    public interface IChessGameFactory
    {
        IChessGame CreateGame(bool isPlayerWhite);
    }
}
