using ChessDefinitions;
using Chess;

namespace ChessGameRepresentation
{
    /// <summary>
    /// Represents a chess game state
    /// </summary>
    public class ChessGameState : IChessGame
    {

        readonly ChessBoardState boardState;
        readonly PlayersDescriptor playersDescriptor;

        public ChessGameState(IPlayersDescriptor playersDescriptor)
        {
            boardState = new ChessBoardState();
            this.playersDescriptor = new PlayersDescriptor(playersDescriptor.WhitePlayerType, playersDescriptor.BlackPlayerType, playersDescriptor.AIPlayerDifficulty);
        }
        public ChessGameState(IPlayersDescriptor playersDescriptor, string fen)
        {
            boardState = new ChessBoardState(fen);
            this.playersDescriptor = new PlayersDescriptor(playersDescriptor.WhitePlayerType, playersDescriptor.BlackPlayerType, playersDescriptor.AIPlayerDifficulty);
        }

        IPlayersDescriptor IChessGame.Players => playersDescriptor;

        IChessBoard IChessGame.Board => boardState;

        IChessGameSnapshot IChessGame.GetSnapshot()
        {
            return new ChessGameSnapshot(boardState.GetFen(), boardState.GetCurrentState(), playersDescriptor);
        }
    }

    public class ChessGameStateFactory : IChessGameFactory
    {
        IChessGame IChessGameFactory.CreateGame(IPlayersDescriptor playersDescriptor)
        {
            return new ChessGameState(playersDescriptor);
        }

        IChessGame IChessGameFactory.CreateGameFromFen(IPlayersDescriptor playersDescriptor, string fen)
        {
            return new ChessGameState(playersDescriptor, fen);
        }
    }
}