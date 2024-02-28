using ChessDefinitions;
using ChessGameRepresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class ChessGameStateTests
    {
        [Fact]
        public void GetCurrentState_InProgressGame_ShouldReturnInProgress()
        {
            // Arrange
            var gameState = new ChessGameState(isPlayerWhite: true);

            // Act
            var currentState = gameState.GetCurrentState();

            // Assert
            currentState.ShouldBe(IChessGame.GameState.InProgress);
        }

        [Fact]
        public void IsWhiteMove_WhiteTurn_ShouldReturnTrue()
        {
            // Arrange
            var gameState = new ChessGameState(isPlayerWhite: true);

            // Act
            var isWhiteMove = gameState.IsWhiteMove();

            // Assert
            isWhiteMove.ShouldBeTrue();
        }

        [Fact]
        public void MakeMove_ValidMove_ShouldReturnTrueAndChangeState()
        {
            // Arrange
            var gameState = new ChessGameState(isPlayerWhite: true);
            var initialFen = gameState.GetFen();
            var validMove = "e2e4";

            // Act
            var moveResult = gameState.MakeMove(validMove);

            // Assert
            moveResult.ShouldBeTrue();
            var currentState = gameState.GetCurrentState();
            currentState.ShouldBe(IChessGame.GameState.InProgress);
            gameState.GetFen().ShouldNotBe(initialFen);
        }

        [Fact]
        public void MakeMove_InvalidMove_ShouldReturnFalseAndNotChangeState()
        {
            // Arrange
            var gameState = new ChessGameState(isPlayerWhite: true);
            var initialFen = gameState.GetFen();
            var invalidMove = "e2e5"; // Invalid

            // Act
            var moveResult = gameState.MakeMove(invalidMove);

            // Assert
            moveResult.ShouldBeFalse();
            var currentState = gameState.GetCurrentState();
            currentState.ShouldBe(IChessGame.GameState.InProgress);
            gameState.GetFen().ShouldBe(initialFen);
        }

        [Fact]
        public void MakeMove_InvalidMove_ExceptionThrown_ShouldReturnFalseAndNotChangeState()
        {
            // Arrange
            var gameState = new ChessGameState(isPlayerWhite: true);
            var initialFen = gameState.GetFen();
            var invalidMove = "e2e"; // Invalid format

            // Act
            var moveResult = gameState.MakeMove(invalidMove);

            // Assert
            moveResult.ShouldBeFalse();
            var currentState = gameState.GetCurrentState();
            currentState.ShouldBe(IChessGame.GameState.InProgress);
            gameState.GetFen().ShouldBe(initialFen);
        }

        [Fact]
        public void MakeMove_InvalidMove_DoesNotChangeState()
        {
            // Arrange
            var gameState = new ChessGameState(isPlayerWhite: true);
            var initialFen = gameState.GetFen();
            var invalidMove = "e2e5"; // Invalid

            // Act
            var moveResult = gameState.MakeMove(invalidMove);

            // Assert
            moveResult.ShouldBeFalse();
            var currentState = gameState.GetCurrentState();
            currentState.ShouldBe(IChessGame.GameState.InProgress);
            gameState.GetFen().ShouldBe(initialFen); 
        }


    }
}
