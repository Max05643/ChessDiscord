using ChessDefinitions;
using ChessGameRepresentation;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using static ChessGameControllerImplementation.IGamesStorage;

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

    IChessGamesController.MoveRequestResult IChessGamesController.MakeMove(string gameId, string move, out IChessGameSnapshot? currentGameState)
    {
        currentGameState = null;


        try
        {
            using var gameHandler = gamesStorage.AcquireGame(gameId);
            if (gameHandler != null)
            {
                if (!moveValidator.Validate(move))
                {
                    currentGameState = ChessGameSnapshot.ConstructFromChessGameState(gameHandler.Game);
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
                    currentGameState = ChessGameSnapshot.ConstructFromChessGameState(gameHandler.Game);
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
            currentGameState = null;
            logger.LogError("Error in MakeMove: {e}", e);
            return IChessGamesController.MoveRequestResult.InternalError;
        }
    }

    IChessGamesController.RemoveGameResult IChessGamesController.RemoveGame(string gameId)
    {
        try
        {
            gamesStorage.RemoveGame(gameId);
            return IChessGamesController.RemoveGameResult.Success;
        }
        catch (Exception e)
        {
            logger.LogError("Error in RemoveGame: {e}", e);
            return IChessGamesController.RemoveGameResult.InternalError;
        }
    }

    IChessGamesController.NewGameResult IChessGamesController.StartNewGame(string gameId, bool isPlayerWhite, out IChessGameSnapshot? currentGameState)
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
            currentGameState = ChessGameSnapshot.ConstructFromChessGameState(newGameState);
            return IChessGamesController.NewGameResult.Success;
        }
        catch (Exception e)
        {
            logger.LogError("Error in StartNewGame: {e}", e);
            currentGameState = null;
            return IChessGamesController.NewGameResult.InternalError;
        }

        
    }
}
