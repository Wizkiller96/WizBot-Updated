﻿using Discord;
using Discord.Commands;
using WizBot.Core.Services;
using System.Threading.Tasks;
using WizBot.Common.Attributes;
using WizBot.Modules.Administration.Services;

namespace WizBot.Modules.Administration
{
    public partial class Administration
    {
        [Group]
        public class GameChannelCommands : WizBotSubmodule<GameVoiceChannelService>
        {
            [WizBotCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [UserPerm(GuildPerm.Administrator)]
            [BotPerm(GuildPerm.MoveMembers)]
            public async Task GameVoiceChannel()
            {
                var vch = ((IGuildUser)ctx.User).VoiceChannel;

                if (vch == null)
                {
                    await ReplyErrorLocalizedAsync("not_in_voice").ConfigureAwait(false);
                    return;
                }
                var id = _service.ToggleGameVoiceChannel(ctx.Guild.Id, vch.Id);

                if (id == null)
                {
                    await ReplyConfirmLocalizedAsync("gvc_disabled").ConfigureAwait(false);
                }
                else
                {
                    _service.GameVoiceChannels.Add(vch.Id);
                    await ReplyConfirmLocalizedAsync("gvc_enabled", Format.Bold(vch.Name)).ConfigureAwait(false);
                }
            }
        }
    }
}
