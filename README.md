---

# AntiDefen Discord Bot

AntiDefen is a Discord bot designed to monitor voice state updates on your Discord server. When a user self-deafens in a voice channel, the bot sends a configurable kick message and then moves the user out of the channel. The bot uses a custom logging framework to record detailed log messages, including caller information, to help with debugging and monitoring.

## Features

- **Voice State Monitoring:** Detects when a user self-deafens in a voice channel.
- **Automated Action:** Sends a kick message to the user and moves them from the voice channel.
- **Custom Logging:** Uses a robust, production-ready custom logger that includes caller info, debug mode, and file write retry logic.
- **Configuration Management:** Reads settings such as the Discord token and kick message from a configuration file.

## Prerequisites

- [.NET 8+](https://dotnet.microsoft.com/download) (or later)
- [Discord.Net](https://github.com/discord-net/Discord.Net) library
- [Newtonsoft.Json](https://www.newtonsoft.com/json) for configuration management

## Installation

1. **Clone the Repository:**

   ```bash
   git clone https://github.com/jacobwasbeast/AntiDefen.git
   cd AntiDefen
   ```

2. **Restore Dependencies:**

   Use the .NET CLI to restore the required packages:

   ```bash
   dotnet restore
   ```

3. **Build the Project:**

   ```bash
   dotnet build
   ```

## Configuration

The bot reads its configuration from a file named `config.json` located in the same directory as the executable. Below is an example of what your `config.json` might look like:

```json
{
  "discordToken": "YOUR_DISCORD_BOT_TOKEN_HERE",
  "kickMessage": "You have been kicked from the voice channel because you self-deafened."
}
```

> **Note:** Ensure that the `config.json` file is copied to your output directory (e.g., `bin/Debug/net8.0/`) when running the application.

## Running the Bot

After building the project and setting up your configuration file, run the bot using the .NET CLI:

```bash
dotnet run
```

The bot will log into Discord, and you'll see log messages indicating the bot's activity in the console as well as in the `logs` directory.

## Logging

The bot uses a custom logging framework that:

- Logs messages with different severity levels (INFO, WARNING, ERROR, DEBUG).
- Includes caller information (class name, method, file, and line number) in each log message.
- Writes logs to file(s) located in the `logs` directory. This directory is configurable via the logger.
- Implements retry logic when writing to log files in case of file locks or other IO issues.

### Example Log Message

```
[2025-02-12 15:30:45] [INFO] Bot started successfully. | AntiDefen.Program.Main (Line 23 in Program.cs)
```

## Code Overview

The main entry point of the application is in `Program.cs`, which:

- Initializes the `ConfigManager` to load the bot's configuration.
- Configures the `CustomLogger` (setting debug mode, caller info, and log directory).
- Creates and starts the `DiscordSocketClient` to connect to Discord.
- Hooks into the `UserVoiceStateUpdated` event to monitor when a user self-deafens.
- Logs various events (information, errors, warnings) using the `CustomLogger`.

## Contributing

Contributions are welcome! If you find a bug or have a feature request, please open an issue or submit a pull request.

## License

This project is licensed under the [MIT License](LICENSE).
