using ChessDefinitions;
using ChessGameRepresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlServerStorage
{
    /// <summary>
    /// Provides methods to convert IChessGame to string
    /// </summary>
    internal static class Conversions
    {
        public static string? IChessGameToString(IChessGame? chessGame)
        {
            if (chessGame == null)
                return null;
            else
                return $"{chessGame.Players.WhitePlayerType == PlayerType.Human}@{chessGame.Board.GetFen()}@{(int)chessGame.Players.AIPlayerDifficulty}";
        }

        public static IChessGame? StringToIChessGame(string? str, IChessGameFactory chessGameFactory)
        {
            if (str == null)
                return null;
            else
            {
                var splitted = str.Split('@');
                var isPlayerWhite = bool.Parse(splitted[0]);
                var fen = splitted[1];
                var difficulty = (AIDifficulty)int.Parse(splitted[2]);

                var game = chessGameFactory.CreateGameFromFen(new PlayersDescriptor(isPlayerWhite ? PlayerType.Human : PlayerType.AI, !isPlayerWhite ? PlayerType.Human : PlayerType.AI, difficulty),
                    fen);

                return game;
            }
        }
    }
}
