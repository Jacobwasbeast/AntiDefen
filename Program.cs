using System;
using System.Threading;
using System.Threading.Tasks;
using AntiDefen.Config;
using AntiDefen.Logger;
using Discord;
using Discord.WebSocket;

namespace AntiDefen
{
    class Program
    {
        private static DiscordSocketClient _client;
        public static ConfigManager ConfigManager;

        public static async Task Main(string[] args)
        {
            ConfigManager = new ConfigManager();
            CustomLogger.DebugMode = true;
            CustomLogger.IncludeCallerInfo = true;
            CustomLogger.SetLogDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"));
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
                CustomLogger.LogInformation("Bot started successfully.");
            }
            catch (Exception ex)
            {
                CustomLogger.LogError(ex, "Failed to login");
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
                    CustomLogger.LogInformation("User {0} self-deafened in channel {1}", user.Username, guildUser.VoiceChannel.Name);
                    await SendMessageAsync(user);
                    await guildUser.VoiceChannel.Guild.MoveAsync(guildUser, null);
                    CustomLogger.LogInformation("User {0} has been moved from channel {1}", user.Username, guildUser.VoiceChannel.Name);
                }
            }
        }

        private static async Task SendMessageAsync(SocketUser user)
        {
            try
            {
                await user.SendMessageAsync(ConfigManager.GetConfig()?.kickMessage);
                CustomLogger.LogInformation("Sent kick message to user {0}", user.Username);
            }
            catch (Exception ex)
            {
                CustomLogger.LogWarning("Failed to send message to {0}: {1}", user.Username, ex.Message);
            }
        }

        private static Task LogAsync(LogMessage log)
        {
            switch (log.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    CustomLogger.LogError(log.Message);
                    break;
                case LogSeverity.Warning:
                    CustomLogger.LogWarning(log.Message);
                    break;
                case LogSeverity.Info:
                    CustomLogger.LogInformation(log.Message);
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    CustomLogger.LogDebug(log.Message);
                    break;
                default:
                    CustomLogger.LogInformation(log.Message);
                    break;
            }
            return Task.CompletedTask;
        }
    }
}
