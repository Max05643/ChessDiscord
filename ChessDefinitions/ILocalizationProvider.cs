using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDefinitions
{
    /// <summary>
    /// Provides a way to obtain localized text
    /// </summary>
    public interface ILocalizationProvider
    {
        /// <summary>
        /// Returns a localized string by its id
        /// </summary>
        public string GetLocalizedText(string lineId);
    }
}
