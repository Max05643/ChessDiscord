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
        public PlayersDescriptor(PlayerType whitePlayerType, PlayerType blackPlayerType)
        {
            this.whitePlayerType = whitePlayerType;
            this.blackPlayerType = blackPlayerType;
        }

        public PlayersDescriptor(bool isPlayerWhite)
        {
            whitePlayerType = isPlayerWhite ? PlayerType.Human : PlayerType.AI;
            blackPlayerType = !isPlayerWhite ? PlayerType.Human : PlayerType.AI;
        }
        PlayerType IPlayersDescriptor.WhitePlayerType => whitePlayerType;

        PlayerType IPlayersDescriptor.BlackPlayerType => blackPlayerType;
    }
}
