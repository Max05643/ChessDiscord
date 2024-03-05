using ChessDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGameControllerImplementation
{
    /// <summary>
    /// Provides a way to convert AIDifficulty to actual chess ai's search depth
    /// </summary>
    internal static class DifficultyToDepthConverter
    {
        public static int ConvertDifficultyToDepth(this AIDifficulty difficulty)
        {
            return (int)difficulty;
        }
    }
}
