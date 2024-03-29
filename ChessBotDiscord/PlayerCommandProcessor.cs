﻿using ChessDefinitions;

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


        private string ConstructDescriptionFromGame(IChessGameSnapshot game)
        {
            return $"{localizationProvider.GetLocalizedText("WhitePlayer")}-{localizationProvider.GetLocalizedText(game.Players.WhitePlayerType == PlayerType.Human ? "HumanPlayer" : "AIPlayer")}\n{localizationProvider.GetLocalizedText("BlackPlayer")}-{localizationProvider.GetLocalizedText(game.Players.BlackPlayerType == PlayerType.Human ? "HumanPlayer" : "AIPlayer")}";
        }

        IPlayersCommandProcessor.CommandResult IPlayersCommandProcessor.MakeMove(string gameId, string playerMove)
        {
            var result = chessGamesController.MakeMove(gameId, playerMove, out IChessGameSnapshot? gameState);
            var output = new IPlayersCommandProcessor.CommandResult();

            if (gameState != null)
            {
                output.ChessGameFen = gameState.Fen;
                output.Description = ConstructDescriptionFromGame(gameState);
            }
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
                if (gameState!.State == GameState.InProgress)
                {
                    output.Message = localizationProvider.GetLocalizedText("MakeYourMove");
                }
                else
                {
                    switch (gameState.State)
                    {
                        case GameState.Stalemate:
                            output.Message = localizationProvider.GetLocalizedText("TieMessage");
                            break;
                        case GameState.BlackWon:
                            output.Message = gameState.Players.WhitePlayerType == PlayerType.Human ? localizationProvider.GetLocalizedText("LoseMessage") : localizationProvider.GetLocalizedText("WinMessage");
                            break;
                        case GameState.WhiteWon:
                            output.Message = gameState.Players.WhitePlayerType == PlayerType.Human ? localizationProvider.GetLocalizedText("LoseMessage") : localizationProvider.GetLocalizedText("WinMessage");
                            break;
                    }
                }
            }

            return output;
        }

        IPlayersCommandProcessor.CommandResult IPlayersCommandProcessor.RemoveGame(string gameId)
        {
            var result = chessGamesController.RemoveGame(gameId);

            if (result == IChessGamesController.RemoveGameResult.Success)
            {
                return new IPlayersCommandProcessor.CommandResult() { Message = localizationProvider.GetLocalizedText("GameRemoved") };
            }
            else
            {
                return new IPlayersCommandProcessor.CommandResult() { Message = localizationProvider.GetLocalizedText("InternalErrorMessage") };
            }

        }

        IPlayersCommandProcessor.CommandResult IPlayersCommandProcessor.StartNewGame(string gameId, bool isPlayerWhite, AIDifficulty aIDifficulty)
        {
            var result = chessGamesController.StartNewGame(gameId, isPlayerWhite, aIDifficulty, out IChessGameSnapshot? gameState);

            if (result == IChessGamesController.NewGameResult.Success)
            {
                return new IPlayersCommandProcessor.CommandResult() { Message = localizationProvider.GetLocalizedText("GameStarted"), ChessGameFen = gameState!.Fen, Description = ConstructDescriptionFromGame(gameState) };
            }
            else
            {
                return new IPlayersCommandProcessor.CommandResult() { Message = localizationProvider.GetLocalizedText("InternalErrorMessage") };
            }


        }
    }
}
