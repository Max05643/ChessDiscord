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
        string IBoardVisualizer.GameToUrl(string chessGameFen)
        {
            string fenEnc = chessGameFen.Replace(" ", "%20");
            return $"https://chessboardimage.com/{fenEnc}.png";
        }
    }
}
