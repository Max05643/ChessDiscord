using ChessDefinitions;
using ChessGameRepresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class MoveValidatorTests
    {
        private readonly IMoveValidator _moveValidator;

        public MoveValidatorTests()
        {
            _moveValidator = new MoveValidator();
        }

        [Theory]
        [InlineData("e2e4")]
        [InlineData("a1h8")]
        [InlineData("b2b3")]
        public void Validate_ValidMoves_ShouldReturnTrue(string move)
        {
            // Act
            var isValid = _moveValidator.Validate(move);

            // Assert
            isValid.ShouldBeTrue();
        }

        [Theory]
        [InlineData("e2e")]
        [InlineData("e2e45")]
        [InlineData("e9e4")]
        [InlineData("e2x4")]
        [InlineData("e2-e4")]
        [InlineData("e2xe4")]
        [InlineData("invalid")]
        [InlineData("a9b1")]
        [InlineData("i1h8")]
        [InlineData("b9b3")]
        public void Validate_InvalidMoves_ShouldReturnFalse(string move)
        {
            // Act
            var isValid = _moveValidator.Validate(move);

            // Assert
            isValid.ShouldBeFalse();
        }
    }
}
