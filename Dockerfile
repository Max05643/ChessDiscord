FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source


COPY *.sln .
COPY BoardVisualizer/*.csproj ./BoardVisualizer/
COPY ChessBotDiscord/*.csproj ./ChessBotDiscord/
COPY ChessDefinitions/*.csproj ./ChessDefinitions/
COPY ChessGameControllerImplementation/*.csproj ./ChessGameControllerImplementation/
COPY ChessGameRepresentation/*.csproj ./ChessGameRepresentation/
COPY StockfishWrapper/*.csproj ./StockfishWrapper/
COPY Tests/*.csproj ./Tests/
COPY SqlServerStorage/*.csproj ./SqlServerStorage/
RUN dotnet restore


COPY BoardVisualizer/. ./BoardVisualizer/
COPY ChessBotDiscord/. ./ChessBotDiscord/
COPY ChessDefinitions/. ./ChessDefinitions/
COPY ChessGameControllerImplementation/. ./ChessGameControllerImplementation/
COPY ChessGameRepresentation/. ./ChessGameRepresentation/
COPY StockfishWrapper/. ./StockfishWrapper/
COPY Tests/. ./Tests/
COPY SqlServerStorage/. ./SqlServerStorage/
WORKDIR /source/ChessBotDiscord
RUN dotnet publish -c release -o /app --no-restore



FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /app
COPY --from=build /app ./

RUN apt-get update && apt-get install -y stockfish

ENV PathToStockFish "/usr/bin/stockfish"
ENV DOTNET_ENVIRONMENT Production
ENTRYPOINT ["dotnet", "ChessBotDiscord.dll"]