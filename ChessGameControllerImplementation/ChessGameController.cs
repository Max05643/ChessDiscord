using ChessDefinitions;
using System.Collections.Concurrent;

namespace ChessGameControllerImplementation;

/// <summary>
/// In-memory controller of multiply chess games
/// </summary>
public class ChessGameController : IChessGamesController
{

    /// <summary>
    /// Represents a stored chess game
    /// </summary>
    class ChessGameEntry
    {
        public IChessGame GameState { get; set; }
        
        /// <summary>
        /// Used for thread-safety
        /// </summary>
        public object Locker { get; set; } = new object();

        public ChessGameEntry(IChessGame gameState)
        {
            GameState = gameState;
        }
    }


    /// <summary>
    /// Currently stored games
    /// </summary>
    readonly ConcurrentDictionary<string, ChessGameEntry> currentGames = new();
    readonly IChessAI chessAI;
    readonly IChessGameFactory gameFactory;
    readonly IMoveValidator moveValidator;
    public ChessGameController(IChessAI chessAI, IChessGameFactory gameFactory, IMoveValidator moveValidator)
    {
        this.chessAI = chessAI;
        this.gameFactory = gameFactory;
        this.moveValidator = moveValidator;
    }

    IChessGamesController.MoveRequestResult IChessGamesController.MakeMove(string gameId, string move, out IChessGame? currentGameState)
    {
        currentGameState = null;

        if (currentGames.TryGetValue(gameId, out ChessGameEntry? currentGameEntry))
        {
            lock (currentGameEntry.Locker)
            {

                currentGameState = currentGameEntry.GameState.Clone();

                if (!moveValidator.Validate(move))
                {
                    return IChessGamesController.MoveRequestResult.WrongFormat;
                }
                
                if (currentGameEntry.GameState.GetCurrentState() != IChessGame.GameState.InProgress)
                {
                    return IChessGamesController.MoveRequestResult.GameAlreadyEnded;
                }

                if (currentGameEntry.GameState.MakeMove(move))
                {
                    chessAI.GetNextMove(currentGameEntry.GameState.GetFen(), out string? aiMove);
                    currentGameEntry.GameState.MakeMove(aiMove!);


                    currentGameState = currentGameEntry.GameState.Clone();
                    return IChessGamesController.MoveRequestResult.Success;
                }
                else
                {
                    return IChessGamesController.MoveRequestResult.IllegalMove;
                }

            }
        }
        else
        {
            return IChessGamesController.MoveRequestResult.GameNotFound;
        }
    }

    IChessGame IChessGamesController.StartNewGame(string gameId, bool isPlayerWhite)
    {
        var newGameState = gameFactory.CreateGame(isPlayerWhite);

        if (!isPlayerWhite)
        {
            chessAI.GetNextMove(newGameState.GetFen(), out string? aiMove);
            newGameState.MakeMove(aiMove!);
        }

        currentGames.AddOrUpdate(gameId, new ChessGameEntry(newGameState), (gameId, oldVal) => new ChessGameEntry(newGameState));


        return newGameState.Clone();
    }
}
