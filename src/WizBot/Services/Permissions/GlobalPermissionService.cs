﻿using WizBot.DataStructures.ModuleBehaviors;
using WizBot.Services.Database.Models;
using System.Collections.Concurrent;
using System.Linq;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace WizBot.Services.Permissions
{
    public class GlobalPermissionService : ILateBlocker
    {
        public readonly ConcurrentHashSet<string> BlockedModules;
        public readonly ConcurrentHashSet<string> BlockedCommands;

        public GlobalPermissionService(BotConfig bc)
        {
            BlockedModules = new ConcurrentHashSet<string>(bc.BlockedModules.Select(x => x.Name));
            BlockedCommands = new ConcurrentHashSet<string>(bc.BlockedCommands.Select(x => x.Name));
        }

        public async Task<bool> TryBlockLate(DiscordSocketClient client, IUserMessage msg, IGuild guild, IMessageChannel channel, IUser user, string moduleName, string commandName)
        {
            await Task.Yield();
            commandName = commandName.ToLowerInvariant();

            if (commandName != "resetglobalperms" &&
                (BlockedCommands.Contains(commandName) ||
                BlockedModules.Contains(moduleName.ToLowerInvariant())))
            {
                return true;
                //return new ExecuteCommandResult(cmd, null, SearchResult.FromError(CommandError.Exception, $"Command or module is blocked globally by the bot owner."));
            }
            return false;
        }
    }
}
