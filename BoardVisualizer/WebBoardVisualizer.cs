using ChessDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardVisualizer
{
    /// <summary>
    /// Uses chessboardimage.com to visualize the game
    /// </summary>
    public class WebBoardVisualizer : IBoardVisualizer
    {
        string IBoardVisualizer.GameToUrl(IChessGame chessGame)
        {
            var fen = chessGame.GetFen();
            fen = fen.Replace(" ", "%20");
            return $"https://chessboardimage.com/{fen}.png";
        }
    }
}
