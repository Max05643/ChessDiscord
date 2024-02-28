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
        readonly IChessGamesController chessGamesController;
        readonly IBoardVisualizer boardVisualizer;
        readonly ILocalizationProvider localizationProvider;
        readonly ILogger<DiscordBotController> logger;

        public DiscordBotController(IConfiguration configuration, IChessGamesController chessGamesController, IBoardVisualizer boardVisualizer, ILocalizationProvider localizationProvider, ILogger<DiscordBotController> logger)
        {
            botToken = configuration["BotToken"]!;
            this.chessGamesController = chessGamesController;
            this.boardVisualizer = boardVisualizer;
            this.localizationProvider = localizationProvider;
            this.logger = logger;
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
            var result = chessGamesController.MakeMove(command.ChannelId.ToString()!, playerMove, out IChessGame? gameState);

            if (result == IChessGamesController.MoveRequestResult.InternalError)
            {
                await command.ModifyOriginalResponseAsync((settings) => settings.Content = localizationProvider.GetLocalizedText("InternalErrorMessage"));
            }
            else if (result == IChessGamesController.MoveRequestResult.WrongFormat)
            {
                await command.ModifyOriginalResponseAsync((settings) => settings.Content = localizationProvider.GetLocalizedText("WrongMoveFormatMessage"));
            }
            else if (result == IChessGamesController.MoveRequestResult.GameNotFound)
            {
                await command.ModifyOriginalResponseAsync((settings) => settings.Content = localizationProvider.GetLocalizedText("GameNotFoundMessage"));
            }
            else if (result == IChessGamesController.MoveRequestResult.IllegalMove)
            {
                await command.ModifyOriginalResponseAsync((settings) => settings.Content = localizationProvider.GetLocalizedText("IllegalMoveMessage"));
            }
            else if (result == IChessGamesController.MoveRequestResult.GameAlreadyEnded)
            {
                await command.ModifyOriginalResponseAsync((settings) => settings.Content = localizationProvider.GetLocalizedText("GameAlreadyEndedMessage"));
            }
            else if (result == IChessGamesController.MoveRequestResult.Success)
            {
                var img = boardVisualizer.GameToUrl(gameState!);

                if (gameState!.GetCurrentState() == IChessGame.GameState.InProgress)
                {
                    await command.ModifyOriginalResponseAsync((settings) => settings.Embed = new EmbedBuilder().WithImageUrl(img).WithColor(Color.Red).WithTitle(localizationProvider.GetLocalizedText("MakeYourMove")).Build());
                }
                else
                {
                    string gameResultDesc = "";

                    switch (gameState!.GetCurrentState())
                    {
                        case IChessGame.GameState.Stalemate:
                            gameResultDesc = localizationProvider.GetLocalizedText("TieMessage");
                            break;
                        case IChessGame.GameState.BlackWon:
                            gameResultDesc = gameState!.IsPlayerWhite ? localizationProvider.GetLocalizedText("LoseMessage") : localizationProvider.GetLocalizedText("WinMessage");
                            break;
                        case IChessGame.GameState.WhiteWon:
                            gameResultDesc = !gameState!.IsPlayerWhite ? localizationProvider.GetLocalizedText("LoseMessage") : localizationProvider.GetLocalizedText("WinMessage");
                            break;
                    }

                    await command.ModifyOriginalResponseAsync((settings) => settings.Embed = new EmbedBuilder().WithImageUrl(img).WithColor(Color.Red).WithTitle(localizationProvider.GetLocalizedText("EndMessage")).WithDescription(gameResultDesc).Build());
                }
            }



        }

        /// <summary>
        /// Starts a new game
        /// </summary>
        async Task NewGame(SocketSlashCommand command)
        {
            await command.DeferAsync();

            var isPlayerWhite = (bool)(command.Data.Options.First(opt => opt.Name == "isplayerwhite").Value);
            var gameState = chessGamesController.StartNewGame(command.ChannelId.ToString()!, isPlayerWhite);

            var img = boardVisualizer.GameToUrl(gameState);

            await command.ModifyOriginalResponseAsync((settings) => settings.Embed = new EmbedBuilder().WithImageUrl(img).WithColor(Color.Red).WithTitle(localizationProvider.GetLocalizedText("GameStarted")).Build());

        }

        /// <summary>
        /// Removes an existing game
        /// </summary>
        async Task RemoveGame(SocketSlashCommand command)
        {
            await command.DeferAsync();

            chessGamesController.RemoveGame(command.ChannelId.ToString()!);

            await command.ModifyOriginalResponseAsync((settings) => settings.Embed = new EmbedBuilder().WithColor(Color.Red).WithTitle(localizationProvider.GetLocalizedText("GameRemoved")).Build());

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
