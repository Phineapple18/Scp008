using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using Log = LabApi.Features.Console.Logger;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using NorthwoodLib.Pools;
using Scp008.Features;
using Utils.NonAllocLINQ;

namespace Scp008.Commands
{
    public class Infect : ICommand, IUsageProvider
    {
        public Infect(string command, string description, string[] aliases)
        {
            translation = Translation.AccessTranslation();
            Command = command ?? _command;
            Description = description;
            Aliases = aliases;
            Usage = new[] { "%player%/all" };
            Log.Debug($"Registered {this.Command} subcommand.", translation.Debug);
        }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (MainClass.Instance == null)
            {
                response = translation.NotEnabled;
                Log.Debug($"Plugin Scp008 is not enabled.", translation.Debug);
                return false;
            }
            if (sender == null)
            {
                response = translation.SenderNull;
                Log.Debug("Command sender is null.", Config.Debug);
                return false;
            }
            if (!sender.HasPermissions("008.infection"))
            {
                response = translation.NoPerms;
                Log.Debug($"Player {sender.LogName} doesn't have required permission to use this command.", Config.Debug);
                return false;
            }
            if (!Round.IsRoundStarted)
            {
                response = translation.RoundNotStarted;
                Log.Debug("This command can't be used before round start.", Config.Debug);
                return false;
            }
            if (arguments.IsEmpty())
            {
                response = $"{Description} {translation.Usage}: {this.DisplayCommandUsage()}.";
                Log.Debug($"Player {sender.LogName} didn't provide arguments for command.", Config.Debug);
                return false;
            }
            List<Player> validPlayers = arguments.At(0).ToLower() == "all" ? Player.List.ToList() : Player.List.Where(p => arguments.Contains(p.PlayerId.ToString())).ToList();
            if (validPlayers.IsEmpty())
            {
                response = translation.NoPlayers;
                Log.Debug($"Player {sender.LogName} provided non-existent player(s).", Config.Debug);
                return false;
            }
            StringBuilder success = StringBuilderPool.Shared.Rent();
            StringBuilder failure = StringBuilderPool.Shared.Rent();
            success.AppendLine(translation.InfectSuccess);
            failure.AppendLine($"{translation.InfectFail}:");
            int[] num = new int[2] { 0, 0 };
            ListExtensions.ForEach(validPlayers, player =>
            {
                if (player.TryInfectWith008(100))
                {
                    num[1]++;
                    return;
                }
                failure.AppendLine($"- {player.Nickname}");
                num[0]++;
                Log.Debug($"Player {player.Nickname} is ineligible to be infected with Scp008.", Config.Debug);
            });
            success.Replace("%num%", num[1].ToString());
            failure.Replace("%num%", num[0].ToString());
            StringBuilder result = num[1] == 0 ? failure : num[0] == 0 ? success : success.Append(failure);
            response = StringBuilderPool.Shared.ToStringReturn(result).TrimEnd(Array.Empty<char>());
            Log.Debug($"Player {sender.LogName} infected successfully ({num[1]}) and unsuccessfully ({num[0]}) players with Scp008.", Config.Debug);
            return true;
        }

        internal const string _command = "infect";
        internal const string _description = "Infect chosen player(s) with Scp008. Separate entries with space.";
        internal static readonly string[] _aliases = new[] { "i" };
        private readonly Translation translation;

        public string Command { get; }
        public string Description { get; }
        public string[] Aliases { get; }
        public string[] Usage { get; }
        private static Config Config => MainClass.Instance.pluginConfig;
    }
}
