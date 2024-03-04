using ChessBotDiscord;
using ChessDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class PlayerCommandProcessorTests
    {
        [Fact]
        public void MakeMove_InternalError_ShouldReturnInternalErrorMessage()
        {
            // Arrange
            string gameId = "game123";
            string playerMove = "e2e4";
            var mockChessGamesController = new Mock<IChessGamesController>();
            var mockLocalizationProvider = new Mock<ILocalizationProvider>();

            mockChessGamesController.Setup(c => c.MakeMove(gameId, playerMove, out It.Ref<IChessGameSnapshot?>.IsAny)).Returns(IChessGamesController.MoveRequestResult.InternalError);
            mockLocalizationProvider.Setup(lp => lp.GetLocalizedText("InternalErrorMessage")).Returns("Internal error occurred.");

            IPlayersCommandProcessor commandProcessor = new PlayerCommandProcessor(mockChessGamesController.Object, mockLocalizationProvider.Object);


            // Act
            var result = commandProcessor.MakeMove(gameId, playerMove);

            // Assert
            result.Message.ShouldBe("Internal error occurred.");
            mockChessGamesController.Verify(controller => controller.MakeMove(It.IsAny<string>(), It.IsAny<string>(), out It.Ref<IChessGameSnapshot?>.IsAny));
        }

        [Fact]
        public void MakeMove_WrongMoveFormat_ShouldReturnWrongMoveFormatMessage()
        {
            // Arrange
            string gameId = "game123";
            string playerMove = "invalidMove";
            var mockChessGamesController = new Mock<IChessGamesController>();
            var mockLocalizationProvider = new Mock<ILocalizationProvider>();

            mockChessGamesController.Setup(c => c.MakeMove(gameId, playerMove, out It.Ref<IChessGameSnapshot?>.IsAny)).Returns(IChessGamesController.MoveRequestResult.WrongFormat);
            mockLocalizationProvider.Setup(lp => lp.GetLocalizedText("WrongMoveFormatMessage")).Returns("Invalid move format.");


            IPlayersCommandProcessor commandProcessor = new PlayerCommandProcessor(mockChessGamesController.Object, mockLocalizationProvider.Object);


            // Act
            var result = commandProcessor.MakeMove(gameId, playerMove);

            // Assert
            result.Message.ShouldBe("Invalid move format.");
            mockChessGamesController.Verify(controller => controller.MakeMove(It.IsAny<string>(), It.IsAny<string>(), out It.Ref<IChessGameSnapshot?>.IsAny));
        }

        [Fact]
        public void MakeMove_GameNotFound_ShouldReturnGameNotFoundMessage()
        {
            // Arrange
            string gameId = "nonExistentGame";
            string playerMove = "e2e4";
            var mockChessGamesController = new Mock<IChessGamesController>();
            var mockLocalizationProvider = new Mock<ILocalizationProvider>();

            mockChessGamesController.Setup(c => c.MakeMove(gameId, playerMove, out It.Ref<IChessGameSnapshot?>.IsAny)).Returns(IChessGamesController.MoveRequestResult.GameNotFound);
            mockLocalizationProvider.Setup(lp => lp.GetLocalizedText("GameNotFoundMessage")).Returns("Game not found.");


            IPlayersCommandProcessor commandProcessor = new PlayerCommandProcessor(mockChessGamesController.Object, mockLocalizationProvider.Object);


            // Act
            var result = commandProcessor.MakeMove(gameId, playerMove);

            // Assert
            result.Message.ShouldBe("Game not found.");
            mockChessGamesController.Verify(controller => controller.MakeMove(It.IsAny<string>(), It.IsAny<string>(), out It.Ref<IChessGameSnapshot?>.IsAny));
        }

        [Fact]
        public void MakeMove_IllegalMove_ShouldReturnIllegalMoveMessage()
        {
            // Arrange
            string gameId = "game123";
            string playerMove = "e2e4";
            var mockChessGamesController = new Mock<IChessGamesController>();
            var mockLocalizationProvider = new Mock<ILocalizationProvider>();

            mockChessGamesController.Setup(c => c.MakeMove(gameId, playerMove, out It.Ref<IChessGameSnapshot?>.IsAny)).Returns(IChessGamesController.MoveRequestResult.IllegalMove);
            mockLocalizationProvider.Setup(lp => lp.GetLocalizedText("IllegalMoveMessage")).Returns("Illegal move.");


            IPlayersCommandProcessor commandProcessor = new PlayerCommandProcessor(mockChessGamesController.Object, mockLocalizationProvider.Object);


            // Act
            var result = commandProcessor.MakeMove(gameId, playerMove);

            // Assert
            result.Message.ShouldBe("Illegal move.");
            mockChessGamesController.Verify(controller => controller.MakeMove(It.IsAny<string>(), It.IsAny<string>(), out It.Ref<IChessGameSnapshot?>.IsAny));
        }

        [Fact]
        public void MakeMove_GameAlreadyEnded_ShouldReturnGameAlreadyEndedMessage()
        {
            // Arrange
            string gameId = "game123";
            string playerMove = "e2e4";
            var mockChessGamesController = new Mock<IChessGamesController>();
            var mockLocalizationProvider = new Mock<ILocalizationProvider>();

            mockChessGamesController.Setup(c => c.MakeMove(gameId, playerMove, out It.Ref<IChessGameSnapshot?>.IsAny)).Returns(IChessGamesController.MoveRequestResult.GameAlreadyEnded);
            mockLocalizationProvider.Setup(lp => lp.GetLocalizedText("GameAlreadyEndedMessage")).Returns("Game has already ended.");



            IPlayersCommandProcessor commandProcessor = new PlayerCommandProcessor(mockChessGamesController.Object, mockLocalizationProvider.Object);


            // Act
            var result = commandProcessor.MakeMove(gameId, playerMove);

            // Assert
            result.Message.ShouldBe("Game has already ended.");
            mockChessGamesController.Verify(controller => controller.MakeMove(It.IsAny<string>(), It.IsAny<string>(), out It.Ref<IChessGameSnapshot?>.IsAny));
        }

        [Fact]
        public void MakeMove_Success_ShouldReturnMakeYourMoveMessageAndGameFen()
        {
            // Arrange
            string gameId = "game123";
            string playerMove = "e2e4";
            var mockChessGamesController = new Mock<IChessGamesController>();
            var mockLocalizationProvider = new Mock<ILocalizationProvider>();

            var gameStateMock = new Mock<IChessGameSnapshot>();
            gameStateMock.SetupGet(gs => gs.State).Returns(GameState.InProgress);
            gameStateMock.SetupGet(gs => gs.Fen).Returns("fenString");
            gameStateMock.SetupGet(gs => gs.Players.WhitePlayerType).Returns(PlayerType.Human);
            gameStateMock.SetupGet(gs => gs.Players.BlackPlayerType).Returns(PlayerType.AI);

            mockChessGamesController.Setup(c => c.MakeMove(gameId, playerMove, out It.Ref<IChessGameSnapshot?>.IsAny)).Callback((string gameId, string playerMove, out IChessGameSnapshot? game) => { game = gameStateMock.Object; }).Returns(IChessGamesController.MoveRequestResult.Success);
            mockLocalizationProvider.Setup(lp => lp.GetLocalizedText("MakeYourMove")).Returns("Make your move.");

            IPlayersCommandProcessor commandProcessor = new PlayerCommandProcessor(mockChessGamesController.Object, mockLocalizationProvider.Object);

            // Act
            var result = commandProcessor.MakeMove(gameId, playerMove);

            // Assert
            result.Message.ShouldBe("Make your move.");
            result.ChessGameFen.ShouldBe("fenString");
            mockChessGamesController.Verify(controller => controller.MakeMove(It.IsAny<string>(), It.IsAny<string>(), out It.Ref<IChessGameSnapshot?>.IsAny));
        }

    }
}

