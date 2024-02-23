using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using ChessDefinitions;

namespace ChessBotDiscord
{
    /// <summary>
    /// Represents a running instance of Discord bot
    /// </summary>
    public class DiscordBotController
    {

        private DiscordSocketClient? client;
        private readonly string botToken;
        private bool initialized = false;

        private readonly IChessGamesController chessGamesController;


        public DiscordBotController(string botToken, IChessGamesController chessGamesController)
        {
            this.botToken = botToken;
            this.chessGamesController = chessGamesController;
        }

        /// <summary>
        /// Is bot ready?
        /// </summary>
        public bool IsReady()
        {
            return initialized;
        }

        /// <summary>
        /// Should be called to start the bot
        /// </summary>
        public void Initialize()
        {

            client = new DiscordSocketClient(new DiscordSocketConfig() { GatewayIntents = GatewayIntents.AllUnprivileged ^ GatewayIntents.GuildInvites ^ GatewayIntents.GuildScheduledEvents }); ;
            

            client.Log += LogBotOutput;
            client.Ready += InitializeCommands;
            client.SlashCommandExecuted += SlashCommandHandler;

            client.LoginAsync(TokenType.Bot, botToken).Wait();
            client.StartAsync();
        }

        /// <summary>
        /// Handles bot's commands initialization
        /// </summary>
        async Task InitializeCommands()
        {
            var newGameCommand = new SlashCommandBuilder();
            newGameCommand.WithName("newgame");
            newGameCommand.WithDescription("Starts a new chess game");
            newGameCommand.AddOption("isplayerwhite", ApplicationCommandOptionType.Boolean, "Is player plays as white", isRequired: true);

            var moveCommand = new SlashCommandBuilder();
            moveCommand.WithName("move");
            moveCommand.WithDescription("Makes a move");
            moveCommand.AddOption("move", ApplicationCommandOptionType.String, "Move in notation 'e2e4'", isRequired: true);

            await client!.CreateGlobalApplicationCommandAsync(newGameCommand.Build());
            await client!.CreateGlobalApplicationCommandAsync(moveCommand.Build());


            initialized = true;
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
                await command.ModifyOriginalResponseAsync((settings) => settings.Content = "Error =(");
            }
            else if (result == IChessGamesController.MoveRequestResult.WrongFormat)
            {
                await command.ModifyOriginalResponseAsync((settings) => settings.Content = "Wrong move format");
            }
            else if (result == IChessGamesController.MoveRequestResult.GameNotFound)
            {
                await command.ModifyOriginalResponseAsync((settings) => settings.Content = "Game not found");
            }
            else if (result == IChessGamesController.MoveRequestResult.IllegalMove)
            {
                await command.ModifyOriginalResponseAsync((settings) => settings.Content = "This move is illegal");
            }
            else if (result == IChessGamesController.MoveRequestResult.GameAlreadyEnded)
            {
                await command.ModifyOriginalResponseAsync((settings) => settings.Content = "This game is already ended");
            }
            else if (result == IChessGamesController.MoveRequestResult.Success)
            {
                if (gameState!.GetCurrentState() == IChessGame.GameState.InProgress)
                {
                    await command.ModifyOriginalResponseAsync((settings) => settings.Content = "Make your move!\n" + $"```{gameState!.ToAscii()}```");
                }
                else
                {
                    await command.ModifyOriginalResponseAsync((settings) => settings.Content = "It is the end\n" + gameState!.ToAscii());
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

            string gameTextRepresentation = gameState.ToAscii();

            await command.ModifyOriginalResponseAsync((settings) => settings.Content = "New game started!\n" + $"```{gameTextRepresentation}```");
        }

        Task LogBotOutput(LogMessage message)
        {
            Console.WriteLine($"Log bot output: {message}");
            return Task.CompletedTask;
        }

    }
}
