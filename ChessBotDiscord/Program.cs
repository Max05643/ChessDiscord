using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using ChessDefinitions;
using ChessGameRepresentation;
using ChessBotDiscord;
using ChessGameControllerImplementation;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

IChessAI stockFish = new StockfishWrapper.Stockfish(configuration["PathToStockFish"]!, 100);
IChessGameFactory gameFactory = new ChessGameStateFactory();
IMoveValidator moveValidator = new MoveValidator();
IChessGamesController chessGamesController = new ChessGameController(stockFish, gameFactory, moveValidator);



var token = configuration["BotToken"]!;
var bot = new DiscordBotController(token, chessGamesController);

bot.Initialize();

Console.ReadLine();