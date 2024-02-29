# Chess Discord Bot

Welcome to my Chess Discord Bot repository. This pet project, created by myself in my spare time, allows you to play chess against an AI 🎲🤖 in Discord channels.

## Credits

Used projects:

- **Stockfish**: AI Engine. [stockfishchess.org](https://stockfishchess.org/).
- **Gera Chess Library**: Used for general infrastructure. [GitHub](https://github.com/Geras1mleo/Chess).
- **chessboardimage.com**: Used for generating chessboard images.
- **Discord.NET**: Discord bot framework. [GitHub](https://github.com/discord-net/Discord.Net).

## Usage

To deploy this bot yourself:

1. Prepare a machine with Docker installed
2. Set up environment variables witn `.env` file with the following contents:

```dotenv
ConnectionStrings__SqlStorage="Server=sqlserver;Database=ChessBotStorage;MultipleActiveResultSets=true;User=sa;Password=<your_password>"
BotToken="<your_bot_token>"
```

Replace `<your_password>` with your desired SQL Server password and `<your_bot_token>` with your Discord bot token.

3. Create a Docker Compose file (e.g., `docker-compose.yml`):

```yaml
version: "3"

services:

    bot:
        image: "max05643/chessbotdiscord:latest"
        env_file:
            - .env
        depends_on:
            - sqlserver
    sqlserver:
        image: "mcr.microsoft.com/mssql/server:2019-latest"
        environment:
            SA_PASSWORD: "<your_password>"
            ACCEPT_EULA: "Y"
```

Replace `<your_password>` with your desired SQL Server password.

4. Run `docker-compose up` in your terminal to start the bot and SQL Server containers.

5. Invite the bot to your Discord server using the OAuth2 URL generated from your bot application page on the Discord Developer Portal.

6. Use the `/newgame True|False` command to start a game, where the argument specifies whether a human player plays as white. Use the `/move` command to make a move, and the `/remove` command to remove an existing game.

[![Tests](https://github.com/Max05643/ChessDiscord/actions/workflows/testing.yml/badge.svg)](https://github.com/Max05643/ChessDiscord/actions/workflows/testing.yml) - Testing workflow status for the master branch.

[![Docker](https://github.com/Max05643/ChessDiscord/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/Max05643/ChessDiscord/actions/workflows/dotnet-desktop.yml) - Build and push Docker container workflow status.
