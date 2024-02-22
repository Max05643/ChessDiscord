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

        private readonly IChessGameFactory gameFactory;
        private readonly IChessAI chessAI;
        private bool initialized = false;


        private readonly object locker = new object();


        private IChessGame curentGame;
        private bool isPlayerWhite = true;

        public DiscordBotController(string botToken, IChessGameFactory gameFactory, IChessAI chessAI)
        {
            this.botToken = botToken;
            this.gameFactory = gameFactory;
            this.chessAI = chessAI;
            curentGame = gameFactory.CreateGame();
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
            string gameTextRepresentation = "";

            await command.DeferAsync();

            IChessGame.GameState gameState = IChessGame.GameState.InProgress;

            lock (locker)
            {
                string playerMove = (string)(command.Data.Options.First(opt => opt.Name == "move").Value);

                if (!(isPlayerWhite ^ curentGame.IsWhiteMove()))
                {
                    curentGame.MakeMove(playerMove);

                    if (curentGame.GetCurrentState() == IChessGame.GameState.InProgress)
                    {
                        var result = chessAI.GetNextMove(curentGame.GetFen(), out string? aiMove);

                        if (!result)
                            throw new InvalidOperationException("Chess AI does not work correctly");

                        curentGame.MakeMove(aiMove!);
                    }
                }


                gameTextRepresentation = curentGame.ToAscii();
                gameState = curentGame.GetCurrentState();
            }

            if (gameState == IChessGame.GameState.InProgress)
            {
                await command.ModifyOriginalResponseAsync((settings) => settings.Content = "Make your move!\n" + $"```{gameTextRepresentation}```");
            }
            else
            {
                await command.ModifyOriginalResponseAsync((settings) => settings.Content = "It is the end\n" + gameState.ToString());
            }

        }

        /// <summary>
        /// Starts a new game
        /// </summary>
        async Task NewGame(SocketSlashCommand command)
        {
            string gameTextRepresentation = "";

            await command.DeferAsync();

            lock (locker)
            {
                curentGame = gameFactory.CreateGame();

                var isPlayerWhite = (bool)(command.Data.Options.First(opt => opt.Name == "isplayerwhite").Value);

                if (!isPlayerWhite)
                {
                    var result = chessAI.GetNextMove(curentGame.GetFen(), out string? move);

                    if (!result)
                        throw new InvalidOperationException("Chess AI does not work correctly");

                    curentGame.MakeMove(move!);
                }


                gameTextRepresentation = curentGame.ToAscii();
            }

            await command.ModifyOriginalResponseAsync((settings) => settings.Content = "New game started!\n" + $"```{gameTextRepresentation}```");
        }

        Task LogBotOutput(LogMessage message)
        {
            Console.WriteLine($"Log bot output: {message}");
            return Task.CompletedTask;
        }

    }
}
