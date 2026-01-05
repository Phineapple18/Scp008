using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Log = LabApi.Features.Console.Logger;

using CommandSystem;
using NorthwoodLib.Pools;
using Utils.NonAllocLINQ;

namespace Scp008.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Scp008Parent : ParentCommand
    {
        public Scp008Parent()
        {
            translation = Translation.AccessTranslation();
            Command = translation.Scp008parentCommand ?? _command;
            Description = translation.Scp008parentDescription;
            Aliases = translation.Scp008parentAliases;
            Log.Debug($"Registered {this.Command} parent command.", translation.Debug);
            this.LoadGeneratedCommands();
        }

        public sealed override void LoadGeneratedCommands()
        {
            this.RegisterCommand(new Cure(translation.CureCommand, translation.CureDescription, translation.CureAliases));
            this.RegisterCommand(new Infect(translation.InfectCommand, translation.InfectDescription, translation.InfectAliases));
            this.RegisterCommand(new List(translation.ListCommand, translation.ListDescription, translation.ListAliases));
            Log.Debug($"Registered {this.AllCommands.Count()} command(s) for {this.Command} parent command.", translation.Debug);
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (MainClass.Instance == null)
            {
                response = MainClass.Instance.pluginTranslation.PluginNotEnabled;
                Log.Debug($"Plugin {MainClass.Instance.Name} is not enabled.", MainClass.Instance.pluginTranslation.Debug);
                return false;
            }
            StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
            stringBuilder.AppendLine($"{Description} {translation.Subcommands}:");
            foreach (ICommand command in this.AllCommands)
            {
                stringBuilder.AppendLine($"- {command.Command} | {translation.Aliases}: {(command.Aliases == null || command.Aliases.IsEmpty() ? "" : string.Join(", ", command.Aliases))} | {translation.Description}: {command.Description}");
            }
            response = StringBuilderPool.Shared.ToStringReturn(stringBuilder).TrimEnd(Array.Empty<char>());
            return true;
        }

        internal const string _command = "scp008";
        internal const string _description = "Parent command for handling SCP-008.";
        internal static readonly string[] _aliases = new[] { "008" };
        private readonly Translation translation;

        public override string Command { get; }
        public override string Description { get; }
        public override string[] Aliases { get; }
    }
}
