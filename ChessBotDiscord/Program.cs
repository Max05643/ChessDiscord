using ChessDefinitions;
using ChessGameRepresentation;
using ChessBotDiscord;
using ChessGameControllerImplementation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(services =>
{
    services.AddSingleton<IChessAI, StockfishWrapper.Stockfish>();
    services.AddSingleton<IChessGameFactory, ChessGameStateFactory>();
    services.AddSingleton<IMoveValidator, MoveValidator>();
    services.AddSingleton<IGamesStorage, InMemoryGamesStorage>();
    services.AddSingleton<IChessGamesController, ChessGameController>();
    services.AddSingleton<IBoardVisualizer, BoardVisualizer.WebBoardVisualizer>();
    services.AddSingleton<ILocalizationProvider, ConfigLocalizationProvider>();
    services.AddHostedService<DiscordBotController>();
}

);

var app = builder.Build();

app.Run();