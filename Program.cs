using System;
using System.Threading;
using System.Threading.Tasks;
using AntiDefen.Config;
using Discord;
using Discord.WebSocket;
using MetaLogging;
namespace AntiDefen
{
    class Program
    {
        private static DiscordSocketClient _client;
        public static ConfigManager ConfigManager;

        public static async Task Main(string[] args)
        {
            ConfigManager = new ConfigManager();
            MetaLogger.DebugMode = true;
            MetaLogger.IncludeCallerInfo = true;
            MetaLogger.SetLogDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"));
            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.All | GatewayIntents.MessageContent
            };

            _client = new DiscordSocketClient(config);
            
            _client.Log += LogAsync;
            _client.UserVoiceStateUpdated += OnVoiceStateUpdated;
            
            try
            {
                await _client.LoginAsync(TokenType.Bot, ConfigManager.GetConfig()?.discordToken);
                await _client.StartAsync();
                MetaLogger.LogInformation("Bot started successfully.");
            }
            catch (Exception ex)
            {
                MetaLogger.LogError(ex, "Failed to login");
                Environment.Exit(1);
            }

            await Task.Delay(Timeout.Infinite);
        }

        private static async Task OnVoiceStateUpdated(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
        {
            if (user.IsBot)
                return;
            
            if (newState.VoiceChannel != null && newState.IsSelfDeafened)
            {
                var guildUser = user as IGuildUser;
                if (guildUser?.VoiceChannel != null)
                {
                    MetaLogger.LogInformation("User {0} self-deafened in channel {1}", user.Username, guildUser.VoiceChannel.Name);
                    await SendMessageAsync(user);
                    await guildUser.VoiceChannel.Guild.MoveAsync(guildUser, null);
                    MetaLogger.LogInformation("User {0} has been moved from channel {1}", user.Username, guildUser.VoiceChannel.Name);
                }
            }
        }

        private static async Task SendMessageAsync(SocketUser user)
        {
            try
            {
                await user.SendMessageAsync(ConfigManager.GetConfig()?.kickMessage);
                MetaLogger.LogInformation("Sent kick message to user {0}", user.Username);
            }
            catch (Exception ex)
            {
                MetaLogger.LogWarning("Failed to send message to {0}: {1}", user.Username, ex.Message);
            }
        }

        private static Task LogAsync(LogMessage log)
        {
            switch (log.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    MetaLogger.LogError(log.Message);
                    break;
                case LogSeverity.Warning:
                    MetaLogger.LogWarning(log.Message);
                    break;
                case LogSeverity.Info:
                    MetaLogger.LogInformation(log.Message);
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    MetaLogger.LogDebug(log.Message);
                    break;
                default:
                    MetaLogger.LogInformation(log.Message);
                    break;
            }
            return Task.CompletedTask;
        }
    }
}
