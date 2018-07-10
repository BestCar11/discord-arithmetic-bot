using Discord;
using Discord.WebSocket;
using DiscordArithmeticBot.Modules;
using System;
using System.Threading.Tasks;

namespace DiscordArithmeticBot
{
    internal class Program
    {
        #region Private Fields

        private DiscordSocketClient _client;
        private CommandHandler _handler;

        #endregion Private Fields

        #region Public Methods

        public async Task StartAsync()
        {
            LargeNumber a = new LargeNumber("500");
            LargeNumber b = new LargeNumber("5000");
            LargeNumber sum = LargeNumber.Add(a, b);
            Console.WriteLine(sum.ToString());
            return;
            if (Config.bot.token == "" || Config.bot.token == null) return;
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });
            _client.Log += Log;
            await _client.LoginAsync(TokenType.Bot, Config.bot.token);
            await _client.StartAsync();
            _handler = new CommandHandler();
            await _handler.InitializeAsync(_client);
            await Task.Delay(-1);
        }

        #endregion Public Methods

        #region Private Methods

        private static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();

        private async Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
        }

        #endregion Private Methods
    }
}