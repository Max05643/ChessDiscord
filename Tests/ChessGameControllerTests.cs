using ChessDefinitions;
using ChessGameControllerImplementation;
using ChessGameRepresentation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ChessGameControllerImplementation.IGamesStorage;

namespace Tests
{
    public class ChessGameControllerTests
    {

        [Fact]
        public void StartNewGame_ValidInput_ShouldReturnNewGameState()
        {
            // Arrange

            string gameId = "game123";
            bool isPlayerWhite = true;
            var expectedGameStateMock = new Mock<IChessGame>();
            var snapshotMock = new Mock<IChessGameSnapshot>();
            expectedGameStateMock.Setup(state => state.GetSnapshot()).Returns(snapshotMock.Object);
            expectedGameStateMock.Setup(state => state.Players.WhitePlayerType).Returns(PlayerType.Human);
            expectedGameStateMock.Setup(state => state.Players.BlackPlayerType).Returns(PlayerType.AI);

            var mockChessAI = new Mock<IChessAI>();
            var mockGameFactory = new Mock<IChessGameFactory>();
            var mockMoveValidator = new Mock<IMoveValidator>();
            var mockGamesStorage = new Mock<IGamesStorage>();
            var mockLogger = new Mock<ILogger<ChessGameController>>();
            mockGameFactory.Setup(gf => gf.CreateGame(It.Is<IPlayersDescriptor>(des => des.BlackPlayerType == PlayerType.AI && des.WhitePlayerType == PlayerType.Human))).Returns(expectedGameStateMock.Object);

            IChessGamesController controller = new ChessGameController(
                mockChessAI.Object,
                mockGameFactory.Object,
                mockMoveValidator.Object,
                mockGamesStorage.Object,
                mockLogger.Object);


            // Act
            var result = controller.StartNewGame(gameId, isPlayerWhite, out IChessGameSnapshot? currentGameState);

            // Assert
            currentGameState.ShouldNotBeNull();
            result.ShouldBe(IChessGamesController.NewGameResult.Success);
            mockGamesStorage.Verify(gs => gs.CreateGame(gameId, It.IsAny<IChessGame>()), Times.Once);
        }

        [Fact]
        public void StartNewGame_AIPlaysFirstMove_ShouldMakeAIMove()
        {
            // Arrange
            string gameId = "game123";
            bool isPlayerWhite = false; // AI plays first move
            var expectedGameStateMock = new Mock<IChessGame>();
            var snapshotMock = new Mock<IChessGameSnapshot>();
            expectedGameStateMock.Setup(state => state.GetSnapshot()).Returns(snapshotMock.Object);
            expectedGameStateMock.Setup(state => state.Players.WhitePlayerType).Returns(PlayerType.AI);
            expectedGameStateMock.Setup(state => state.Players.BlackPlayerType).Returns(PlayerType.Human);
            expectedGameStateMock.Setup(state => state.Board.GetFen()).Returns("fen");

            var mockChessAI = new Mock<IChessAI>();
            var mockGameFactory = new Mock<IChessGameFactory>();
            var mockMoveValidator = new Mock<IMoveValidator>();
            var mockGamesStorage = new Mock<IGamesStorage>();
            var mockLogger = new Mock<ILogger<ChessGameController>>();
            mockGameFactory.Setup(gf => gf.CreateGame(It.Is<IPlayersDescriptor>(des => des.BlackPlayerType == PlayerType.Human && des.WhitePlayerType == PlayerType.AI))).Returns(expectedGameStateMock.Object);

            IChessGamesController controller = new ChessGameController(
                mockChessAI.Object,
                mockGameFactory.Object,
                mockMoveValidator.Object,
                mockGamesStorage.Object,
                mockLogger.Object);

            // Act
            var result = controller.StartNewGame(gameId, isPlayerWhite, out IChessGameSnapshot? currentGameState);

            // Assert
            currentGameState.ShouldNotBeNull();
            result.ShouldBe(IChessGamesController.NewGameResult.Success);
            mockChessAI.Verify(ai => ai.GetNextMove("fen", out It.Ref<string?>.IsAny, It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void MakeMove_GameNotFound_ShouldReturnGameNotFound()
        {
            // Arrange
            string gameId = "nonExistentGame";
            string move = "e2e4";


            var mockChessAI = new Mock<IChessAI>();
            var mockGameFactory = new Mock<IChessGameFactory>();
            var mockMoveValidator = new Mock<IMoveValidator>();
            var mockGamesStorage = new Mock<IGamesStorage>();
            var mockLogger = new Mock<ILogger<ChessGameController>>();

            mockGamesStorage.Setup(gs => gs.AcquireGame(gameId)).Returns((IGameHandler)null!);

            IChessGamesController controller = new ChessGameController(
                mockChessAI.Object,
                mockGameFactory.Object,
                mockMoveValidator.Object,
                mockGamesStorage.Object,
                mockLogger.Object);

            // Act
            var result = controller.MakeMove(gameId, move, out IChessGameSnapshot? currentGameState);

            // Assert
            result.ShouldBe(IChessGamesController.MoveRequestResult.GameNotFound);
            currentGameState.ShouldBeNull();
        }

        [Fact]
        public void MakeMove_GameAlreadyEnded_ShouldReturnGameAlreadyEnded()
        {
            // Arrange
            string gameId = "endedGame";
            string move = "e2e4";
            var endedGameMock = new Mock<IGameHandler>();
            endedGameMock.Setup(g => g.Game.Board.GetCurrentState()).Returns(GameState.WhiteWon);

            var mockChessAI = new Mock<IChessAI>();
            var mockGameFactory = new Mock<IChessGameFactory>();
            var mockMoveValidator = new Mock<IMoveValidator>();
            mockMoveValidator.Setup(v => v.Validate(It.IsAny<string>())).Returns(true);
            var mockGamesStorage = new Mock<IGamesStorage>();
            var mockLogger = new Mock<ILogger<ChessGameController>>();

            mockGamesStorage.Setup(gs => gs.AcquireGame(gameId)).Returns(endedGameMock.Object);

            IChessGamesController controller = new ChessGameController(
                mockChessAI.Object,
                mockGameFactory.Object,
                mockMoveValidator.Object,
                mockGamesStorage.Object,
                mockLogger.Object);

            // Act
            var result = controller.MakeMove(gameId, move, out IChessGameSnapshot? currentGameState);

            // Assert
            result.ShouldBe(IChessGamesController.MoveRequestResult.GameAlreadyEnded);
            currentGameState.ShouldBeNull();
        }

        [Fact]
        public void MakeMove_ValidMove_ShouldReturnSuccessAndGameState()
        {
            // Arrange
            string gameId = "game123";
            string move = "e2e4";
            var gameHandlerMock = new Mock<IGameHandler>();
            var gameStateMock = new Mock<IChessGame>();
            var snapshotMock = new Mock<IChessGameSnapshot>();

            var mockChessAI = new Mock<IChessAI>();
            var mockGameFactory = new Mock<IChessGameFactory>();
            var mockMoveValidator = new Mock<IMoveValidator>();
            var mockGamesStorage = new Mock<IGamesStorage>();
            var mockLogger = new Mock<ILogger<ChessGameController>>();

            mockMoveValidator.Setup(mv => mv.Validate(move)).Returns(true);
            gameStateMock.SetupSequence(gs => gs.Board.GetCurrentState())
                .Returns(GameState.InProgress)
                .Returns(GameState.BlackWon);
            gameStateMock.Setup(state => state.Board.MakeMove(It.IsAny<string>())).Returns(true);
            gameStateMock.Setup(state => state.Board.GetFen()).Returns("fen");
            gameStateMock.Setup(state => state.GetSnapshot()).Returns(snapshotMock.Object);

            gameHandlerMock.Setup(gh => gh.Game).Returns(gameStateMock.Object);

            mockGamesStorage.Setup(gs => gs.AcquireGame(gameId)).Returns(gameHandlerMock.Object);

            IChessGamesController controller = new ChessGameController(
                mockChessAI.Object,
                mockGameFactory.Object,
                mockMoveValidator.Object,
                mockGamesStorage.Object,
                mockLogger.Object);

            // Act
            var result = controller.MakeMove(gameId, move, out IChessGameSnapshot? currentGameStateSnapshot);

            // Assert
            result.ShouldBe(IChessGamesController.MoveRequestResult.Success);
            currentGameStateSnapshot.ShouldNotBeNull();
            gameStateMock.Verify(gs => gs.GetSnapshot(), Times.Once);
            gameStateMock.Verify(gs => gs.Board.MakeMove(move), Times.Once);
        }

        [Fact]
        public void MakeMove_InvalidMove_ShouldReturnIllegalMove()
        {
            // Arrange
            string gameId = "game123";
            string move = "e2e5";
            var gameHandlerMock = new Mock<IGameHandler>();
            var gameStateMock = new Mock<IChessGame>();

            var mockChessAI = new Mock<IChessAI>();
            var mockGameFactory = new Mock<IChessGameFactory>();
            var mockMoveValidator = new Mock<IMoveValidator>();
            var mockGamesStorage = new Mock<IGamesStorage>();
            var mockLogger = new Mock<ILogger<ChessGameController>>();

            mockMoveValidator.Setup(mv => mv.Validate(move)).Returns(true);
            gameStateMock.Setup(gs => gs.Board.GetCurrentState()).Returns(GameState.InProgress);
            gameStateMock.Setup(gs => gs.Board.MakeMove(move)).Returns(false);

            gameHandlerMock.Setup(gh => gh.Game).Returns(gameStateMock.Object);
            mockGamesStorage.Setup(gs => gs.AcquireGame(gameId)).Returns(gameHandlerMock.Object);

            IChessGamesController controller = new ChessGameController(
                mockChessAI.Object,
                mockGameFactory.Object,
                mockMoveValidator.Object,
                mockGamesStorage.Object,
                mockLogger.Object);

            // Act
            var result = controller.MakeMove(gameId, move, out IChessGameSnapshot? currentSnapshot);

            // Assert
            result.ShouldBe(IChessGamesController.MoveRequestResult.IllegalMove);
            currentSnapshot.ShouldBeNull();
            gameStateMock.Verify(gs => gs.Board.MakeMove(move), Times.Once);
        }


    }
}
