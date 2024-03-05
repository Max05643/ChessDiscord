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
            IChessGame gameState = new ChessGameState(new PlayersDescriptor(true, AIDifficulty.VeryEasy));

            // Act
            var currentState = gameState.Board.GetCurrentState();

            // Assert
            currentState.ShouldBe(GameState.InProgress);
        }

        [Fact]
        public void IsWhiteMove_WhiteTurn_ShouldReturnTrue()
        {
            // Arrange
            IChessGame gameState = new ChessGameState(new PlayersDescriptor(true, AIDifficulty.VeryEasy));

            // Act
            var isWhiteMove = gameState.Board.IsWhiteMove();

            // Assert
            isWhiteMove.ShouldBeTrue();
        }

        [Fact]
        public void MakeMove_ValidMove_ShouldReturnTrueAndChangeState()
        {
            // Arrange
            IChessGame gameState = new ChessGameState(new PlayersDescriptor(true, AIDifficulty.VeryEasy));
            var initialFen = gameState.Board.GetFen();
            var validMove = "e2e4";

            // Act
            var moveResult = gameState.Board.MakeMove(validMove);

            // Assert
            moveResult.ShouldBeTrue();
            var currentState = gameState.Board.GetCurrentState();
            currentState.ShouldBe(GameState.InProgress);
            gameState.Board.GetFen().ShouldNotBe(initialFen);
        }

        [Fact]
        public void MakeMove_InvalidMove_ShouldReturnFalseAndNotChangeState()
        {
            // Arrange
            IChessGame gameState = new ChessGameState(new PlayersDescriptor(true, AIDifficulty.VeryEasy));
            var initialFen = gameState.Board.GetFen();
            var invalidMove = "e2e5"; // Invalid

            // Act
            var moveResult = gameState.Board.MakeMove(invalidMove);

            // Assert
            moveResult.ShouldBeFalse();
            var currentState = gameState.Board.GetCurrentState();
            currentState.ShouldBe(GameState.InProgress);
            gameState.Board.GetFen().ShouldBe(initialFen);
        }

        [Fact]
        public void MakeMove_InvalidMove_ExceptionThrown_ShouldReturnFalseAndNotChangeState()
        {
            // Arrange
            IChessGame gameState = new ChessGameState(new PlayersDescriptor(true, AIDifficulty.VeryEasy));
            var initialFen = gameState.Board.GetFen();
            var invalidMove = "e2e"; // Invalid format

            // Act
            var moveResult = gameState.Board.MakeMove(invalidMove);

            // Assert
            moveResult.ShouldBeFalse();
            var currentState = gameState.Board.GetCurrentState();
            currentState.ShouldBe(GameState.InProgress);
            gameState.Board.GetFen().ShouldBe(initialFen);
        }

        [Fact]
        public void MakeMove_InvalidMove_DoesNotChangeState()
        {
            // Arrange
            IChessGame gameState = new ChessGameState(new PlayersDescriptor(true, AIDifficulty.VeryEasy));
            var initialFen = gameState.Board.GetFen();
            var invalidMove = "e2e5"; // Invalid

            // Act
            var moveResult = gameState.Board.MakeMove(invalidMove);

            // Assert
            moveResult.ShouldBeFalse();
            var currentState = gameState.Board.GetCurrentState();
            currentState.ShouldBe(GameState.InProgress);
            gameState.Board.GetFen().ShouldBe(initialFen); 
        }


    }
}
