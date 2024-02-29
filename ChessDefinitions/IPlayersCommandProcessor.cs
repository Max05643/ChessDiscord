using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ChessDefinitions.IChessGamesController;

namespace ChessDefinitions
{
    /// <summary>
    /// Provides a way to process players' commands and return text messages and current game board status
    /// </summary>
    public interface IPlayersCommandProcessor
    {
        /// <summary>
        /// Represents a visual result of user's command
        /// </summary>
        public class CommandResult
        {
            public string Message { get; set; } = string.Empty;

            public string Description { get; set; } = string.Empty;
            public string? ChessGameFen { get; set; } = null;
        }

        /// <summary>
        /// Processes a command to remove the game
        /// </summary>
        /// <param name="gameId">Unique id for the game</param>
        CommandResult RemoveGame(string gameId);

        /// <summary>
        /// Processes a command to start new game
        /// </summary>
        /// <param name="gameId">Unique id for the game</param>
        /// <param name="isPlayerWhite">Is player playing as white?</param>
        CommandResult StartNewGame(string gameId, bool isPlayerWhite);

        /// <summary>
        /// Processes a command to make a move 
        /// </summary>
        /// <param name="gameId">Unique id for the game</param>
        CommandResult MakeMove(string gameId, string move);


    }
}
