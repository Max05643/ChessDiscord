using ChessDefinitions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessBotDiscord
{
    /// <summary>
    /// Provides a localization from configuration
    /// </summary>
    public class ConfigLocalizationProvider : ILocalizationProvider
    {
        readonly IConfiguration configuration;
        public ConfigLocalizationProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        string ILocalizationProvider.GetLocalizedText(string lineId)
        {
            var text = configuration[$"Localization:{lineId}"];
            if (text == null)
            {
                throw new InvalidOperationException($"Can not get {lineId} from configuration");
            }
            else
            {
                return text;
            }
        }
    }
}
