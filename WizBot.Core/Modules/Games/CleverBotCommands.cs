﻿using Discord;
using Discord.Commands;
using WizBot.Core.Services;
using System;
using System.Threading.Tasks;
using WizBot.Common.Attributes;
using WizBot.Modules.Games.Services;
using WizBot.Modules.Games.Common.ChatterBot;

namespace WizBot.Modules.Games
{
    public partial class Games
    {
        [Group]
        public class ChatterBotCommands : WizBotSubmodule<ChatterBotService>
        {
            private readonly DbService _db;

            public ChatterBotCommands(DbService db)
            {
                _db = db;
            }

            [WizBotCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [UserPerm(GuildPerm.ManageMessages)]
            public async Task Cleverbot()
            {
                var channel = (ITextChannel)ctx.Channel;

                if (_service.ChatterBotGuilds.TryRemove(channel.Guild.Id, out _))
                {
                    using (var uow = _db.GetDbContext())
                    {
                        uow.GuildConfigs.SetCleverbotEnabled(ctx.Guild.Id, false);
                        await uow.SaveChangesAsync();
                    }
                    await ReplyConfirmLocalizedAsync("cleverbot_disabled").ConfigureAwait(false);
                    return;
                }

                _service.ChatterBotGuilds.TryAdd(channel.Guild.Id, new Lazy<IChatterBotSession>(() => _service.CreateSession(), true));

                using (var uow = _db.GetDbContext())
                {
                    uow.GuildConfigs.SetCleverbotEnabled(ctx.Guild.Id, true);
                    await uow.SaveChangesAsync();
                }

                await ReplyConfirmLocalizedAsync("cleverbot_enabled").ConfigureAwait(false);
            }
        }

       
    }
}