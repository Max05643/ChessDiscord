using ChessDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ChessGameRepresentation
{
    /// <summary>
    /// Validates move accordingly to a format that ChessGameState consumes
    /// </summary>
    public class MoveValidator : IMoveValidator
    {
        private readonly Regex moveRegex = new Regex(@"^[a-h][1-8][a-h][1-8]$");
        bool IMoveValidator.Validate(string move)
        {
            return moveRegex.IsMatch(move);
        }
    }
}
