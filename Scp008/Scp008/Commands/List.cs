using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Log = LabApi.Features.Console.Logger;

using CommandSystem;
using LabApi.Features.Permissions;
using Scp008.Features;

namespace Scp008.Commands
{
    public class List : ICommand
    {
        public List(string command, string description, string[] aliases)
        {
            Command = command ?? _command;
            Description = description;
            Aliases = aliases;
            Log.Info($"Registered {this.Command} subcommand.");
        }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender == null)
            {
                response = Translation.SenderNull;
                Log.Debug("Command sender is null.", Config.Debug);
                return false;
            }
            if (!sender.HasPermissions("008.list"))
            {
                response = Translation.NoPerms;
                Log.Debug($"Player {sender.LogName} doesn't have required permission to use this command.", Config.Debug);
                return false;
            }
            response = $"{Translation.ListSuccess.Replace("%count%", Scp008Extensions.List.Count().ToString())}:\n- {string.Join("\n- ", Scp008Extensions.List.Select(p => p.Nickname))}";
            return true;
        }

        internal const string _command = "list";
        internal const string _description = "Print a list of all players infected with Scp008.";
        internal static readonly string[] _aliases = new[] { "l" };

        public string Command { get; }
        public string Description { get; }
        public string[] Aliases { get; }
        private Config Config => MainClass.Instance.pluginConfig;
        private Translation Translation => MainClass.Instance.pluginTranslation;
    }
}
