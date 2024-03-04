using Chess;
using ChessDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGameRepresentation
{
    public class ChessBoardState : IChessBoard
    {
        readonly ChessBoard board;

        public ChessBoardState()
        {
            board = new ChessBoard();
        }

        /// <summary>
        /// Constructs a new game from fen
        /// </summary>
        public ChessBoardState(string fen)
        {
            board = ChessBoard.LoadFromFen(fen);
        }

        public GameState GetCurrentState()
        {
            if (board.IsEndGame)
            {
                return board.EndGame!.EndgameType == EndgameType.Checkmate ? (board.EndGame.WonSide == PieceColor.Black ? GameState.BlackWon : GameState.WhiteWon) : GameState.Stalemate;
            }
            else
            {
                return GameState.InProgress;
            }
        }

        public string GetFen()
        {
            return board.ToFen();
        }

        public bool IsWhiteMove()
        {
            if (GetCurrentState() == GameState.InProgress)
            {
                return board.Turn == PieceColor.White;
            }
            else
            {
                return false;
            }
        }
        public bool MakeMove(string move)
        {
            bool isValid;
            try
            {
                isValid = board.IsValidMove(move);

            }
            catch (ChessException)
            {
                return false;
            }

            if (isValid)
            {
                board.Move(move);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
