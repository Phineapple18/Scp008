using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using Log = LabApi.Features.Console.Logger;
using LabApi.Features.Permissions;
using Scp008.Features;

namespace Scp008.Commands
{
    public class List : ICommand
    {
        public List(string command, string description, string[] aliases)
        {
            translation = Translation.AccessTranslation();
            Command = command ?? _command;
            Description = description;
            Aliases = aliases;
            Log.Debug($"Loaded {this.Command} subcommand.", translation.Debug);
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
            if (!sender.HasPermissions("008.list"))
            {
                response = translation.NoPerms;
                Log.Debug($"Player {sender.LogName} doesn't have required permission to use this command.", Config.Debug);
                return false;
            }
            response = $"{translation.ListSuccess.Replace("%count%", Scp008Extensions.List.Count().ToString())}:\n- {string.Join("\n- ", Scp008Extensions.List.Select(p => p.Nickname))}";
            return true;
        }

        internal const string _command = "list";
        internal const string _description = "Print a list of all players infected with Scp008.";
        internal static readonly string[] _aliases = new[] { "l" };
        private readonly Translation translation;

        public string Command { get; }
        public string Description { get; }
        public string[] Aliases { get; }
        private static Config Config => MainClass.Instance.pluginConfig;
    }
}
