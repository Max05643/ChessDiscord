using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using ChessDefinitions;
using ChessGameRepresentation;
using ChessBotDiscord;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var token = configuration["BotToken"]!;
var bot = new DiscordBotController(token, new ChessGameStateFactory(), new StockfishWrapper.Stockfish(configuration["PathToStockFish"]!, 100));

bot.Initialize();

Console.ReadLine();