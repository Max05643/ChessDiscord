using ChessDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGameRepresentation
{
    public class PlayersDescriptor : IPlayersDescriptor
    {
        readonly PlayerType whitePlayerType;
        readonly PlayerType blackPlayerType;
        readonly AIDifficulty aIDifficulty;

        public PlayersDescriptor(PlayerType whitePlayerType, PlayerType blackPlayerType, AIDifficulty aIDifficulty)
        {
            this.whitePlayerType = whitePlayerType;
            this.blackPlayerType = blackPlayerType;
            this.aIDifficulty = aIDifficulty;
        }

        public PlayersDescriptor(bool isPlayerWhite, AIDifficulty aIDifficulty)
        {
            whitePlayerType = isPlayerWhite ? PlayerType.Human : PlayerType.AI;
            blackPlayerType = !isPlayerWhite ? PlayerType.Human : PlayerType.AI;
            this.aIDifficulty = aIDifficulty;
        }
        PlayerType IPlayersDescriptor.WhitePlayerType => whitePlayerType;

        PlayerType IPlayersDescriptor.BlackPlayerType => blackPlayerType;

        AIDifficulty IPlayersDescriptor.AIPlayerDifficulty => aIDifficulty;
    }
}
