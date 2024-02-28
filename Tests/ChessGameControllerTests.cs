﻿using ChessDefinitions;
using ChessGameControllerImplementation;
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
            expectedGameStateMock.Setup(state => state.Clone()).Returns(expectedGameStateMock.Object);

            var mockChessAI = new Mock<IChessAI>();
            var mockGameFactory = new Mock<IChessGameFactory>();
            var mockMoveValidator = new Mock<IMoveValidator>();
            var mockGamesStorage = new Mock<IGamesStorage>();
            var mockLogger = new Mock<ILogger<ChessGameController>>();
            mockGameFactory.Setup(gf => gf.CreateGame(isPlayerWhite)).Returns(expectedGameStateMock.Object);

            IChessGamesController controller = new ChessGameController(
                mockChessAI.Object,
                mockGameFactory.Object,
                mockMoveValidator.Object,
                mockGamesStorage.Object,
                mockLogger.Object);


            // Act
            var result = controller.StartNewGame(gameId, isPlayerWhite);

            // Assert
            result.ShouldNotBeNull();
            mockGamesStorage.Verify(gs => gs.CreateGame(gameId, It.IsAny<IChessGame>()), Times.Once);
        }

        [Fact]
        public void StartNewGame_AIPlaysFirstMove_ShouldMakeAIMove()
        {
            // Arrange
            string gameId = "game123";
            bool isPlayerWhite = false; // AI plays first move
            var expectedGameStateMock = new Mock<IChessGame>();
            expectedGameStateMock.Setup(state => state.Clone()).Returns(expectedGameStateMock.Object);

            var mockChessAI = new Mock<IChessAI>();
            var mockGameFactory = new Mock<IChessGameFactory>();
            var mockMoveValidator = new Mock<IMoveValidator>();
            var mockGamesStorage = new Mock<IGamesStorage>();
            var mockLogger = new Mock<ILogger<ChessGameController>>();
            mockGameFactory.Setup(gf => gf.CreateGame(isPlayerWhite)).Returns(expectedGameStateMock.Object);

            IChessGamesController controller = new ChessGameController(
                mockChessAI.Object,
                mockGameFactory.Object,
                mockMoveValidator.Object,
                mockGamesStorage.Object,
                mockLogger.Object);

            // Act
            var result = controller.StartNewGame(gameId, isPlayerWhite);

            // Assert
            result.ShouldNotBeNull();
            mockChessAI.Verify(ai => ai.GetNextMove(It.IsAny<string>(), out It.Ref<string?>.IsAny), Times.Once);
        }

        [Fact]
        public void MakeMove_GameNotFound_ShouldReturnGameNotFound()
        {
            // Arrange
            string gameId = "nonExistentGame";
            string move = "e2e4";
            IChessGame? currentGameState;

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
            var result = controller.MakeMove(gameId, move, out currentGameState);

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
            IChessGame? currentGameState;
            var endedGameMock = new Mock<IGameHandler>();
            endedGameMock.Setup(g => g.Game.GetCurrentState()).Returns(IChessGame.GameState.WhiteWon);

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
            var result = controller.MakeMove(gameId, move, out currentGameState);

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
            IChessGame? currentGameState;
            var gameHandlerMock = new Mock<IGameHandler>();
            var gameStateMock = new Mock<IChessGame>();

            var mockChessAI = new Mock<IChessAI>();
            var mockGameFactory = new Mock<IChessGameFactory>();
            var mockMoveValidator = new Mock<IMoveValidator>();
            var mockGamesStorage = new Mock<IGamesStorage>();
            var mockLogger = new Mock<ILogger<ChessGameController>>();

            mockMoveValidator.Setup(mv => mv.Validate(move)).Returns(true);
            gameStateMock.SetupSequence(gs => gs.GetCurrentState())
                .Returns(IChessGame.GameState.InProgress)
                .Returns(IChessGame.GameState.BlackWon);
            gameStateMock.Setup(state => state.Clone()).Returns(gameStateMock.Object);
            gameStateMock.Setup(state => state.MakeMove(It.IsAny<string>())).Returns(true);

            gameHandlerMock.Setup(gh => gh.Game).Returns(gameStateMock.Object);
            mockGamesStorage.Setup(gs => gs.AcquireGame(gameId)).Returns(gameHandlerMock.Object);

            IChessGamesController controller = new ChessGameController(
                mockChessAI.Object,
                mockGameFactory.Object,
                mockMoveValidator.Object,
                mockGamesStorage.Object,
                mockLogger.Object);

            // Act
            var result = controller.MakeMove(gameId, move, out currentGameState);

            // Assert
            result.ShouldBe(IChessGamesController.MoveRequestResult.Success);
            currentGameState.ShouldNotBeNull();
            currentGameState.ShouldBeSameAs(gameStateMock.Object);
            gameStateMock.Verify(gs => gs.MakeMove(move), Times.Once);
            gameStateMock.Verify(gs => gs.Clone(), Times.Exactly(2)); // Once for AI move, once for returning current state
        }

        [Fact]
        public void MakeMove_InvalidMove_ShouldReturnIllegalMove()
        {
            // Arrange
            string gameId = "game123";
            string move = "e2e5";
            IChessGame? currentGameState;
            var gameHandlerMock = new Mock<IGameHandler>();
            var gameStateMock = new Mock<IChessGame>();

            var mockChessAI = new Mock<IChessAI>();
            var mockGameFactory = new Mock<IChessGameFactory>();
            var mockMoveValidator = new Mock<IMoveValidator>();
            var mockGamesStorage = new Mock<IGamesStorage>();
            var mockLogger = new Mock<ILogger<ChessGameController>>();

            mockMoveValidator.Setup(mv => mv.Validate(move)).Returns(true);
            gameStateMock.Setup(gs => gs.GetCurrentState()).Returns(IChessGame.GameState.InProgress);
            gameStateMock.Setup(gs => gs.MakeMove(move)).Returns(false);

            gameHandlerMock.Setup(gh => gh.Game).Returns(gameStateMock.Object);
            mockGamesStorage.Setup(gs => gs.AcquireGame(gameId)).Returns(gameHandlerMock.Object);

            IChessGamesController controller = new ChessGameController(
                mockChessAI.Object,
                mockGameFactory.Object,
                mockMoveValidator.Object,
                mockGamesStorage.Object,
                mockLogger.Object);

            // Act
            var result = controller.MakeMove(gameId, move, out currentGameState);

            // Assert
            result.ShouldBe(IChessGamesController.MoveRequestResult.IllegalMove);
            currentGameState.ShouldBeNull();
            gameStateMock.Verify(gs => gs.MakeMove(move), Times.Once);
            gameStateMock.Verify(gs => gs.Clone(), Times.Once);
        }


    }
}