using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using ChessDefinitions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ChessGameControllerImplementation;

namespace ChessBotDiscord
{
    /// <summary>
    /// Represents a running instance of Discord bot
    /// </summary>
    public class DiscordBotController : IHostedService
    {

        DiscordSocketClient? client;
        readonly string botToken;
        readonly IBoardVisualizer boardVisualizer;
        readonly ILogger<DiscordBotController> logger;
        readonly IPlayersCommandProcessor playersCommandProcessor;
        readonly ILocalizationProvider localizationProvider;
        public DiscordBotController(IConfiguration configuration, IBoardVisualizer boardVisualizer, ILogger<DiscordBotController> logger, IPlayersCommandProcessor playersCommandProcessor, ILocalizationProvider localizationProvider)
        {
            botToken = configuration["BotToken"]!;
            this.boardVisualizer = boardVisualizer;
            this.logger = logger;
            this.playersCommandProcessor = playersCommandProcessor;
            this.localizationProvider = localizationProvider;
        }


        private Embed BuildEmbed(IPlayersCommandProcessor.CommandResult commandResult)
        {
            var builder = new EmbedBuilder();
            builder.WithDescription(commandResult.Description);
            builder.WithTitle(commandResult.Message);
            if (commandResult.ChessGameFen != null)
            {
                builder.WithImageUrl(boardVisualizer.GameToUrl(commandResult.ChessGameFen));
            }

            return builder.Build();
        }

        /// <summary>
        /// Should be called to start the bot
        /// </summary>
        async Task Initialize()
        {

            client = new DiscordSocketClient(new DiscordSocketConfig() { GatewayIntents = GatewayIntents.AllUnprivileged ^ GatewayIntents.GuildInvites ^ GatewayIntents.GuildScheduledEvents }); ;


            client.Log += LogBotOutput;
            client.Ready += InitializeCommands;
            client.SlashCommandExecuted += SlashCommandHandler;

            await client.LoginAsync(TokenType.Bot, botToken);
            await client.StartAsync();
        }

        /// <summary>
        /// Handles bot's commands initialization
        /// </summary>
        async Task InitializeCommands()
        {
            var newGameCommand = new SlashCommandBuilder();
            newGameCommand.WithName("newgame");
            newGameCommand.WithDescription(localizationProvider.GetLocalizedText("NewGameDesc"));
            newGameCommand.AddOption("isplayerwhite", ApplicationCommandOptionType.Boolean, localizationProvider.GetLocalizedText("IsPlayerWhite"), isRequired: true);

            var moveCommand = new SlashCommandBuilder();
            moveCommand.WithName("move");
            moveCommand.WithDescription(localizationProvider.GetLocalizedText("MoveDesc"));
            moveCommand.AddOption("move", ApplicationCommandOptionType.String, localizationProvider.GetLocalizedText("MoveFormat"), isRequired: true);

            var removeCommand = new SlashCommandBuilder();
            removeCommand.WithName("remove");
            removeCommand.WithDescription(localizationProvider.GetLocalizedText("RemoveDesc"));

            await client!.CreateGlobalApplicationCommandAsync(newGameCommand.Build());
            await client!.CreateGlobalApplicationCommandAsync(moveCommand.Build());
            await client!.CreateGlobalApplicationCommandAsync(removeCommand.Build());
        }

        /// <summary>
        /// Handles slash commands
        /// </summary>
        Task SlashCommandHandler(SocketSlashCommand command)
        {
            switch (command.CommandName)
            {
                case "newgame":
                    Task.Run(() => NewGame(command));
                    break;
                case "move":
                    Task.Run(() => MakeMove(command));
                    break;
                case "remove":
                    Task.Run(() => RemoveGame(command));
                    break;
            }
            return Task.CompletedTask;
        }


        /// <summary>
        /// Makes a move if possible
        /// </summary>
        async Task MakeMove(SocketSlashCommand command)
        {
            await command.DeferAsync();



            string playerMove = (string)(command.Data.Options.First(opt => opt.Name == "move").Value);
            var result = playersCommandProcessor.MakeMove(command.ChannelId.ToString()!, playerMove);

            await command.ModifyOriginalResponseAsync((settings) => settings.Embed = BuildEmbed(result));
        }

        /// <summary>
        /// Starts a new game
        /// </summary>
        async Task NewGame(SocketSlashCommand command)
        {
            await command.DeferAsync();

            var isPlayerWhite = (bool)(command.Data.Options.First(opt => opt.Name == "isplayerwhite").Value);
            var result = playersCommandProcessor.StartNewGame(command.ChannelId.ToString()!, isPlayerWhite);

            await command.ModifyOriginalResponseAsync((settings) => settings.Embed = BuildEmbed(result));

        }

        /// <summary>
        /// Removes an existing game
        /// </summary>
        async Task RemoveGame(SocketSlashCommand command)
        {
            await command.DeferAsync();

            var result = playersCommandProcessor.RemoveGame(command.ChannelId.ToString()!);

            await command.ModifyOriginalResponseAsync((settings) => settings.Embed = BuildEmbed(result));
        }


        Task LogBotOutput(LogMessage message)
        {
            var severity = message.Severity > LogSeverity.Info ? LogLevel.Error : LogLevel.Information;
            logger.Log(severity, "Bot output: {message}", message.ToString());
            return Task.CompletedTask;
        }

        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            return Initialize();
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            return client?.StopAsync() ?? Task.CompletedTask;
        }
    }
}
