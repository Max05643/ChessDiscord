using ChessDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGameRepresentation
{
    /// <summary>
    /// Represents a read-only state of a chess game
    /// </summary>
    public class ChessGameSnapshot : IChessGameSnapshot
    {
        public ChessGameSnapshot(string fen, GameState state, IPlayersDescriptor playersDescriptor)
        {
            this.fen = fen;
            this.state = state;
            this.playersDescriptor = new PlayersDescriptor(playersDescriptor.WhitePlayerType, playersDescriptor.BlackPlayerType);
        }

        readonly string fen;
        readonly GameState state;
        readonly PlayersDescriptor playersDescriptor;

        string IChessGameSnapshot.Fen => fen;

        GameState IChessGameSnapshot.State => state;

        IPlayersDescriptor IChessGameSnapshot.Players => playersDescriptor;
    }
}
