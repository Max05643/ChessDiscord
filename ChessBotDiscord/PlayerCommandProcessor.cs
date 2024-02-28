using ChessDefinitions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ChessBotDiscord
{
    public class PlayerCommandProcessor : IPlayersCommandProcessor
    {

        readonly IChessGamesController chessGamesController;
        readonly ILocalizationProvider localizationProvider;

        public PlayerCommandProcessor(IChessGamesController chessGamesController, ILocalizationProvider localizationProvider)
        {
            this.chessGamesController = chessGamesController;
            this.localizationProvider = localizationProvider;
        }

        IPlayersCommandProcessor.CommandResult IPlayersCommandProcessor.MakeMove(string gameId, string playerMove)
        {
            var result = chessGamesController.MakeMove(gameId, playerMove, out IChessGame? gameState);
            var output = new IPlayersCommandProcessor.CommandResult();

            if (gameState != null)
                output.ChessGameFen = gameState.GetFen();

            if (result == IChessGamesController.MoveRequestResult.InternalError)
            {
                output.Message = localizationProvider.GetLocalizedText("InternalErrorMessage");
            }
            else if (result == IChessGamesController.MoveRequestResult.WrongFormat)
            {
                output.Message = localizationProvider.GetLocalizedText("WrongMoveFormatMessage");
            }
            else if (result == IChessGamesController.MoveRequestResult.GameNotFound)
            {
                output.Message = localizationProvider.GetLocalizedText("GameNotFoundMessage");
            }
            else if (result == IChessGamesController.MoveRequestResult.IllegalMove)
            {
                output.Message = localizationProvider.GetLocalizedText("IllegalMoveMessage");
            }
            else if (result == IChessGamesController.MoveRequestResult.GameAlreadyEnded)
            {
                output.Message = localizationProvider.GetLocalizedText("GameAlreadyEndedMessage");
            }
            else if (result == IChessGamesController.MoveRequestResult.Success)
            {
                if (gameState!.GetCurrentState() == IChessGame.GameState.InProgress)
                {
                    output.Message = localizationProvider.GetLocalizedText("MakeYourMove");
                }
                else
                {
                    switch (gameState!.GetCurrentState())
                    {
                        case IChessGame.GameState.Stalemate:
                            output.Message = localizationProvider.GetLocalizedText("TieMessage");
                            break;
                        case IChessGame.GameState.BlackWon:
                            output.Message = gameState!.IsPlayerWhite ? localizationProvider.GetLocalizedText("LoseMessage") : localizationProvider.GetLocalizedText("WinMessage");
                            break;
                        case IChessGame.GameState.WhiteWon:
                            output.Message = !gameState!.IsPlayerWhite ? localizationProvider.GetLocalizedText("LoseMessage") : localizationProvider.GetLocalizedText("WinMessage");
                            break;
                    }
                }
            }

            return output;
        }

        IPlayersCommandProcessor.CommandResult IPlayersCommandProcessor.RemoveGame(string gameId)
        {
            chessGamesController.RemoveGame(gameId);
            return new IPlayersCommandProcessor.CommandResult() { Message = localizationProvider.GetLocalizedText("GameRemoved") };
        }

        IPlayersCommandProcessor.CommandResult IPlayersCommandProcessor.StartNewGame(string gameId, bool isPlayerWhite)
        {
            var gameState = chessGamesController.StartNewGame(gameId, isPlayerWhite);
            return new IPlayersCommandProcessor.CommandResult() { Message = localizationProvider.GetLocalizedText("GameStarted"), ChessGameFen = gameState.GetFen() };
        }
    }
}
