using ChessDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGameRepresentation
{
    /// <summary>
    /// Represents a read-only state of a chess game
    /// </summary>
    public class ChessGameSnapshot : IChessGameSnapshot
    {
        private ChessGameSnapshot(string fen, IChessGame.GameState state, bool isPlayerWhite)
        {
            this.fen = fen;
            this.state = state;
            this.isPlayerWhite = isPlayerWhite;
        }

        readonly string fen;
        readonly IChessGame.GameState state;
        readonly bool isPlayerWhite;

        string IChessGameSnapshot.Fen => fen;
        IChessGame.GameState IChessGameSnapshot.State => state;
        bool IChessGameSnapshot.IsPlayerWhite => isPlayerWhite;

        public static ChessGameSnapshot ConstructFromChessGameState(IChessGame chessGame)
        {
            return new ChessGameSnapshot(chessGame.GetFen(), chessGame.GetCurrentState(), chessGame.IsPlayerWhite);
        }
    }
}
