using ChessDefinitions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace ChessGameControllerImplementation;

/// <summary>
/// Controller for multiply chess games
/// </summary>
public class ChessGameController : IChessGamesController
{

    readonly IChessAI chessAI;
    readonly IChessGameFactory gameFactory;
    readonly IMoveValidator moveValidator;
    readonly IGamesStorage gamesStorage;
    readonly ILogger<ChessGameController> logger;

    public ChessGameController(IChessAI chessAI, IChessGameFactory gameFactory, IMoveValidator moveValidator, IGamesStorage gamesStorage, ILogger<ChessGameController> logger)
    {
        this.chessAI = chessAI;
        this.gameFactory = gameFactory;
        this.moveValidator = moveValidator;
        this.gamesStorage = gamesStorage;
        this.logger = logger;
    }

    IChessGamesController.MoveRequestResult IChessGamesController.MakeMove(string gameId, string move, out IChessGame? currentGameState)
    {
        currentGameState = null;


        try
        {
            using var gameHandler = gamesStorage.AcquireGame(gameId);
            if (gameHandler != null)
            {
                currentGameState = gameHandler.Game.Clone();

                if (!moveValidator.Validate(move))
                {
                    return IChessGamesController.MoveRequestResult.WrongFormat;
                }

                if (gameHandler.Game.GetCurrentState() != IChessGame.GameState.InProgress)
                {
                    return IChessGamesController.MoveRequestResult.GameAlreadyEnded;
                }

                if (gameHandler.Game.MakeMove(move))
                {
                    chessAI.GetNextMove(gameHandler.Game.GetFen(), out string? aiMove);
                    gameHandler.Game.MakeMove(aiMove!);


                    currentGameState = gameHandler.Game.Clone();
                    return IChessGamesController.MoveRequestResult.Success;
                }
                else
                {
                    return IChessGamesController.MoveRequestResult.IllegalMove;
                }

            }
            else
            {
                return IChessGamesController.MoveRequestResult.GameNotFound;
            }
        }
        catch (Exception e)
        {
            logger.LogError("Error in MakeMove: {e}", e);
            return IChessGamesController.MoveRequestResult.InternalError;
        }
    }

    IChessGame IChessGamesController.StartNewGame(string gameId, bool isPlayerWhite)
    {
        var newGameState = gameFactory.CreateGame(isPlayerWhite);
        try
        {
            if (!isPlayerWhite)
            {
                chessAI.GetNextMove(newGameState.GetFen(), out string? aiMove);
                newGameState.MakeMove(aiMove!);
            }

            gamesStorage.CreateGame(gameId, newGameState);
        }
        catch (Exception e)
        {
            logger.LogError("Error in StartNewGame: {e}", e);
        }

        return newGameState.Clone();
    }
}
