﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDefinitions
{
    /// <summary>
    /// Provides a way to handle players' requests to start new games an interact with existing ones in a thread-safe way
    /// </summary>
    public interface IChessGamesController
    {
        public enum MoveRequestResult
        {
            Success,
            InternalError,
            WrongFormat,
            GameNotFound,
            IllegalMove,
            GameAlreadyEnded
        }

        /// <summary>
        /// Starts new game. Will delete previous one with the same gameId if one exists. Returns new game
        /// </summary>
        /// <param name="gameId">Unique id for the game</param>
        /// <param name="isPlayerWhite">Is player playing as white?</param>
        IChessGame StartNewGame(string gameId, bool isPlayerWhite);

        /// <summary>
        /// Tries to make a move in specified game. Will do nothing if operation is impossible. Will set currentGameState to current state of the game if it is possible  
        /// </summary>
        /// <param name="gameId">Unique id for the game</param>
        /// <param name="currentGameState">Current state of the game if one exists</param>
        MoveRequestResult MakeMove(string gameId, string move, out IChessGame? currentGameState);

    }
}
