using ChessDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlServerStorage.Models
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
                return $"{chessGame.IsPlayerWhite.ToString()}@{chessGame.GetFen()}";
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

                var game = chessGameFactory.CreateGame(isPlayerWhite);
                game.LoadFromFen(fen);

                return game;
            }
        }
    }
}
