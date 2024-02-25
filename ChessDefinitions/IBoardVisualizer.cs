using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDefinitions
{
    /// <summary>
    /// Provides a way to visualize game borad
    /// </summary>
    public interface IBoardVisualizer
    {
        /// <summary>
        /// Converts a povided chess game to an image url
        /// </summary>
        string GameToUrl(IChessGame chessGame);
    }
}
