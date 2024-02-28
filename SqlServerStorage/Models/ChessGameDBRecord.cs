using ChessDefinitions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlServerStorage.Models
{
    /// <summary>
    /// Represents a chess game in database
    /// </summary>
    internal class ChessGameDBRecord
    {
        [Key]
        public string? Id { get; set; }

        public IChessGame? ChessGame { get; set; }

    }
}
