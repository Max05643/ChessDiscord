using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDefinitions
{
    /// <summary>
    /// Provides the information about game's players
    /// </summary>
    public interface IPlayersDescriptor
    {
        /// <summary>
        /// Type of the player playing as white
        /// </summary>
        PlayerType WhitePlayerType { get; }

        /// <summary>
        /// Type of the player playing as black
        /// </summary>
        PlayerType BlackPlayerType { get; }

        /// <summary>
        /// Represents the difficulty of the AI chess player
        /// </summary>
        AIDifficulty AIPlayerDifficulty {  get; }
    }
}
