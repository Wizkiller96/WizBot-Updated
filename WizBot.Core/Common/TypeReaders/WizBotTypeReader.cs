using Discord.Commands;
using Discord.WebSocket;

namespace WizBot.Core.Common.TypeReaders
{
    public abstract class WizBotTypeReader<T> : TypeReader where
        T : class
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _cmds;

        private WizBotTypeReader() { }
        public WizBotTypeReader(DiscordSocketClient client, CommandService cmds)
        {
            _client = client;
            _cmds = cmds;
        }
    }
}