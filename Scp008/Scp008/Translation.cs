using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.IO;

using PluginAPI.Helpers;
using Scp008.Commands;
using Serialization;

namespace Scp008
{
    public class Translation
    {
        [Description("MISCELLANOUS TRANSLATION. Don't translate words between two '%'." +
                     "\n# Message shown upon becoming SCP-0492.")]
        public string SpawnHint { get; set; } = "<voffset=-7em>You can infect others with <color=red>SCP-008</color> by attacking them.</voffset>";

        [Description("Death reason shown, when player dies from the inspection.")]
        public string InfectionDeathReason { get; set; } = "Killed by a mysterious infection.";

        [Description("Hints shown to infected player, when their health drops below the set value. Each hint is shown only once per infection.")]
        public Dictionary<string, float> InfectionMessages { get; set; } = new()
        {
            { "You feel kinda feverish.", 90 },
            { "You feel nauseated.", 55},
            { "You feel very faint.", 20}
        };

        [Description("Hint shown to player when cured.")]
        public string CuredMessage { get; set; } = "Your fever is gone.";

        [Description("COMMANDS\' TRANSLATION. Don't translate words between two '%'." +
                     "\n# Should debug be enabled for command registering/loading?")]
        public bool Debug { get; set; } = false;

        [Description("Translation for Scp008 parent command and its subcommands. Make sure not to duplicate commands or aliases." +
                     "\n# Scp008 parent command.")]
        public string Scp008parentCommand { get; set; } = Scp008Parent._command;

        public string Scp008parentDescription { get; set; } = Scp008Parent._description;

        public string[] Scp008parentAliases { get; set; } = Scp008Parent._aliases;

        [Description("Cure command.")]
        public string CureCommand { get; set; } = Cure._command;

        public string CureDescription { get; set; } = Cure._description;

        public string[] CureAliases { get; set; } = Cure._aliases;

        public string CureSuccess { get; set; } = "Succesfully cured %num% existing player(s) of Scp008";

        public string CureFail { get; set; } = "Command failed for %num% existing player(s) (not infected with Scp008)";

        [Description("Infect command.")]
        public string InfectCommand { get; set; } = Infect._command;

        public string InfectDescription { get; set; } = Infect._description;

        public string[] InfectAliases { get; set; } = Infect._aliases;

        public string InfectSuccess { get; set; } = "Succesfully infected %num% existing player(s) with Scp008";

        public string InfectFail { get; set; } = "Command failed for %num% existing player(s) (already infected with Scp008 or ineligible)";

        [Description("List command.")]
        public string ListCommand { get; set; } = List._command;

        public string ListDescription { get; set; } = List._description;

        public string[] ListAliases { get; set; } = List._aliases;

        public string ListSuccess { get; set; } = "List of players infected with Scp008 (%num%)";

        [Description("Translation for command interface.")]
        public string Aliases { get; set; } = "Aliases";

        public string Description { get; set; } = "Description";

        public string Subcommands { get; set; } = "Available subcommands";

        public string Usage { get; set; } = "Usage";

        [Description("Translation for command responses.")]

        public string DedicatedServer { get; set; } = "You can't use that command on Dedicated Server.";

        public string NoPerms { get; set; } = "You don't have permission to use that command.";

        public string NoPlayers { get; set; } = "Provided player(s) doesn't exist.";

        public string NotEnabled { get; set; } = "Scp008 is not enabled.";

        public string RoundNotStarted { get; set; } = "You can't use that command before round start.";

        public string SenderNull { get; set; } = "Commandsender is null.";

        internal static Translation AccessTranslation()
        {
            string filePath = Path.Combine(File.Exists(globalDll) ? globalTranslation : localTranslation, translationFileName);
            return File.Exists(filePath) ? YamlParser.Deserializer.Deserialize<Translation>(File.ReadAllText(filePath)) : new();
        }

        private static readonly string globalDll = Path.Combine(Paths.GlobalPlugins.Plugins, $"{pluginName}.dll");

        private static readonly string globalTranslation = Path.Combine(Paths.GlobalPlugins.Plugins, pluginName);

        private static readonly string localTranslation = Path.Combine(Paths.LocalPlugins.Plugins, pluginName);

        internal const string pluginName = "Scp008";

        internal const string translationFileName = "translation.yml";
    }
}
