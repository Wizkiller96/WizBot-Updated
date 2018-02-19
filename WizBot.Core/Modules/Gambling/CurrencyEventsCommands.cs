﻿using Discord;
using Discord.Commands;
using WizBot.Extensions;
using WizBot.Core.Services;
using System.Threading.Tasks;
using Discord.WebSocket;
using WizBot.Common.Attributes;
using WizBot.Modules.Gambling.Services;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;
using WizBot.Core.Common;
using WizBot.Core.Services.Database.Models;
using WizBot.Core.Modules.Gambling.Common.Events;
using System;

namespace WizBot.Modules.Gambling
{
    public partial class Gambling
    {
        [Group]
        public class CurrencyEventsCommands : WizBotSubmodule<CurrencyEventsService>
        {
            public enum OtherEvent
            {
                BotListUpvoters
            }

            private readonly DiscordSocketClient _client;
            private readonly IBotCredentials _creds;
            private readonly ICurrencyService _cs;

            public CurrencyEventsCommands(DiscordSocketClient client, ICurrencyService cs, IBotCredentials creds)
            {
                _client = client;
                _creds = creds;
                _cs = cs;
            }

            [WizBotCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [WizBotOptions(typeof(EventOptions))]
            [AdminOnly]
            public async Task EventStart(Event.Type ev, params string[] options)
            {
                var (opts, _) = OptionsParser.Default.ParseFrom(new EventOptions(), options);
                if (!await _service.TryCreateEventAsync(Context.Guild.Id,
                    Context.Channel.Id,
                    ev,
                    opts,
                    GetEmbed
                    ))
                {
                    await ReplyErrorLocalized("start_event_fail").ConfigureAwait(false);
                    return;
                }
            }

            private EmbedBuilder GetEmbed(Event.Type type, EventOptions opts, long currentPot)
            {
                switch (type)
                {
                    case Event.Type.Reaction:
                        return new EmbedBuilder()
                                    .WithOkColor()
                                    .WithTitle(GetText("reaction_title"))
                                    .WithDescription(GetDescription(opts.Amount, currentPot))
                                    .WithFooter(GetText("new_reaction_footer", opts.Hours));
                    default:
                        break;
                }
                throw new ArgumentOutOfRangeException(nameof(type));
            }

            private string GetDescription(long amount, long potSize)
            {
                string potSizeStr = Format.Bold(potSize == 0
                    ? "∞" + _bc.BotConfig.CurrencySign
                    : potSize.ToString() + _bc.BotConfig.CurrencySign);
                return GetText("new_reaction_event",
                                   _bc.BotConfig.CurrencySign,
                                   Format.Bold(amount + _bc.BotConfig.CurrencySign),
                                   potSizeStr);
            }

            [WizBotCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [AdminOnly]
            public async Task EventStart(OtherEvent e)
            {
                switch (e)
                {
#if GLOBAL_WIZBOT
                    case CurrencyEvent.BotListUpvoters:
                        await BotListUpvoters(arg);
                        break;
#endif
                    default:
                        await Task.CompletedTask;
                        return;
                }
            }

            private async Task BotListUpvoters(long amount)
            {
                if (amount <= 0 || string.IsNullOrWhiteSpace(_creds.BotListToken))
                    return;
                string res;
                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Add("Authorization", _creds.BotListToken);
                    res = await http.GetStringAsync($"https://discordbots.org/api/bots/116275390695079945/votes?onlyids=true");
                }
                var ids = JsonConvert.DeserializeObject<ulong[]>(res);
                await _cs.AddBulkAsync(ids, ids.Select(x => "Botlist Upvoter Event"), ids.Select(x => amount), true);
                await ReplyConfirmLocalized("bot_list_awarded",
                    Format.Bold(amount.ToString()),
                    Format.Bold(ids.Length.ToString())).ConfigureAwait(false);
            }

            //    private async Task SneakyGameStatusEvent(ICommandContext context, long num)
            //    {
            //        if (num < 10 || num > 600)
            //            num = 60;

            //        var ev = new SneakyEvent(_cs, _client, _bc, num);
            //        if (!await _service.StartSneakyEvent(ev, context.Message, context))
            //            return;
            //        try
            //        {
            //            var title = GetText("sneakygamestatus_title");
            //            var desc = GetText("sneakygamestatus_desc",
            //                Format.Bold(100.ToString()) + _bc.BotConfig.CurrencySign,
            //                Format.Bold(num.ToString()));
            //            await context.Channel.SendConfirmAsync(title, desc)
            //                .ConfigureAwait(false);
            //        }
            //        catch
            //        {
            //            // ignored
            //        }
            //    }
        }
    }
}
