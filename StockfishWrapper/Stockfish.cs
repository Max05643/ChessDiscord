using ChessDefinitions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockfishWrapper
{
    /// <summary>
    /// Represents an access to stockfish engine. Can be used multiply times
    /// </summary>
    public class Stockfish : IChessAI
    {

        readonly string pathToExecutable;
        const uint maxTries = 1000;

        public Stockfish(IConfiguration configuration)
        {
            pathToExecutable = configuration["PathToStockFish"]!;
        }





        /// <summary>
        /// Returns next best move
        /// </summary>
        /// <param name="currentPositionFen">Current position in FEN</param>
        public bool GetNextMove(string currentPositionFen, out string? move, int depth = 15)
        {
            move = null;

            try
            {
                using var process = new StockfishProcess(pathToExecutable);
                process.WriteLine($"position fen {currentPositionFen}");
                process.WriteLine($"go depth {depth}");

                var tries = 0;
                while (true)
                {
                    if (tries > maxTries)
                    {
                        throw new InvalidOperationException("Stockfish is not responding");
                    }

                    var data = process.ReadLine().Split(' ');

                    if (data[0] == "bestmove")
                    {
                        if (data[1] == "(none)")
                        {
                            return false;
                        }
                        else
                        {
                            move = data[1][..4];
                            return true;
                        }
                    }

                    tries++;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
