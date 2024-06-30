using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using NWAPIPermissionSystem;
using PluginAPI.Core;

namespace Scp008.Commands
{
    public class List : ICommand
    {
        public List(string command, string description, string[] aliases)
        {
            translation = Translation.AccessTranslation();
            commandName = $"{Translation.pluginName}.{this.GetType().Name}";
            Command = !string.IsNullOrWhiteSpace(command) ? command : _command;
            Description = description;
            Aliases = aliases;
            Log.Debug($"Loaded {this.Command} subcommand.", translation.Debug, Translation.pluginName);
        }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (Plugin.Singleton == null)
            {
                response = translation.NotEnabled;
                Log.Debug($"Plugin {Translation.pluginName} is not enabled.", translation.Debug, commandName);
                return false;
            }
            if (sender == null)
            {
                response = translation.SenderNull;
                Log.Debug("Command sender is null.", Config.Debug, commandName);
                return false;
            }
            if (!sender.CheckPermission("008.list"))
            {
                response = translation.NoPerms;
                Log.Debug($"Player {sender.LogName} doesn't have required permission to use this command.", Config.Debug, commandName);
                return false;
            }
            response = $"{translation.ListSuccess.Replace("%num%", Scp008Extensions.List.Count().ToString())}:\n- {string.Join("\n- ", from player in Scp008Extensions.List select player.Nickname)}";
            return true;
        }

        internal const string _command = "list";

        internal const string _description = "Print list of all players infected with Scp008.";

        internal static readonly string[] _aliases = new[] { "l" };

        private readonly string commandName;

        private readonly Translation translation;

        public string Command { get; }
        public string Description { get; }
        public string[] Aliases { get; }
        public bool SanitizeResponse { get; }
        private static Config Config => Plugin.Singleton.pluginConfig;
    }
}
