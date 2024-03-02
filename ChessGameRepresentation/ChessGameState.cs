using ChessDefinitions;
using Chess;

namespace ChessGameRepresentation
{
    /// <summary>
    /// Represents a chess game state
    /// </summary>
    public class ChessGameState : IChessGame
    {

        private ChessBoard board;
        private readonly bool isPlayerWhite;

        public ChessGameState(bool isPlayerWhite)
        {
            board = new ChessBoard();
            this.isPlayerWhite = isPlayerWhite; 
        }

        public bool IsPlayerWhite => isPlayerWhite;

        public IChessGame.GameState GetCurrentState()
        {
            if (board.IsEndGame)
            {
                return board.EndGame!.EndgameType == EndgameType.Checkmate ? (board.EndGame.WonSide == PieceColor.Black ? IChessGame.GameState.BlackWon : IChessGame.GameState.WhiteWon) : IChessGame.GameState.Stalemate;
            }
            else
            {
                return IChessGame.GameState.InProgress;
            }
        }

        public string GetFen()
        {
            return board.ToFen();
        }

        public bool IsWhiteMove()
        {
            if (GetCurrentState() == IChessGame.GameState.InProgress)
            {
                return board.Turn == PieceColor.White;
            }
            else
            {
                return false;
            }
        }

        public void LoadFromFen(string positionFen)
        {
            board = ChessBoard.LoadFromFen(positionFen);
        }

        public bool MakeMove(string move)
        {
            bool isValid = true;
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

        public string ToAscii()
        {
            return board.ToAscii();
        }

    }

    public class ChessGameStateFactory : IChessGameFactory
    {
        IChessGame IChessGameFactory.CreateGame(bool isPlayerWhite)
        {
            return new ChessGameState(isPlayerWhite);
        }
    }
}