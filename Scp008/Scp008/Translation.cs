using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

using LabApi.Loader.Features.Paths;
using LabApi.Features.Wrappers;
using Scp008.Commands;
using Serialization;

namespace Scp008
{
    public class Translation
    {
        [Description("DON'T TRANSLATE WORDS BETWEEN TWO '%'." +
                     "\n# Message shown upon becoming Scp0492.")]
        public string SpawnHint { get; set; } = "<voffset=-7em>You can infect others with <color=red>SCP-008</color> by attacking them.</voffset>";

        [Description("Death reason shown, when player dies from the infection.")]
        public string InfectionDeathReason { get; set; } = "Killed by a mysterious infection.";

        [Description("Custom info of infected players. Leave empty to disable.")]
        public string InfectedCustomInfo { get; set; } = "<color=red>INFECTED</color>";

        [Description("Hints shown to infected player, when their health drops below the set value. Each hint is shown only once per infection.")]
        public Dictionary<int, string> InfectionMessages { get; set; } = new()
        {
            { 90, "You feel kinda feverish." },
            { 55 , "You feel nauseated."},
            { 20 , "You feel very faint."}
        };

        [Description("Hint shown to player when cured.")]
        public string CuredMessage { get; set; } = "Your fever is gone.";

        [Description("Translation for Scp008 parent command and its subcommands. Make sure not to duplicate commands or aliases." +
                     "\n# Scp008 parent command.")]
        public string Scp008parentCommand { get; set; } = Scp008Parent._command;
        public string Scp008parentDescription { get; set; } = Scp008Parent._description;
        public string[] Scp008parentAliases { get; set; } = Scp008Parent._aliases;

        [Description("Cure command.")]
        public string CureCommand { get; set; } = Cure._command;
        public string CureDescription { get; set; } = Cure._description;
        public string[] CureAliases { get; set; } = Cure._aliases;
        public string CureSuccess { get; set; } = "Succesfully cured %count% existing player(s) of Scp008.";
        public string CureFail { get; set; } = "Command failed for %count% existing player(s) (not infected with Scp008)";

        [Description("Infect command.")]
        public string InfectCommand { get; set; } = Infect._command;
        public string InfectDescription { get; set; } = Infect._description;
        public string[] InfectAliases { get; set; } = Infect._aliases;
        public string InfectSuccess { get; set; } = "Succesfully infected %count% existing player(s) with Scp008.";
        public string InfectFail { get; set; } = "Command failed for %count% existing player(s) (already infected with Scp008 or ineligible)";

        [Description("List command.")]
        public string ListCommand { get; set; } = List._command;
        public string ListDescription { get; set; } = List._description;
        public string[] ListAliases { get; set; } = List._aliases;
        public string ListSuccess { get; set; } = "List of players infected with Scp008 (%count%)";

        [Description("Translation for command interface.")]
        public string Aliases { get; set; } = "Aliases";
        public string Description { get; set; } = "Description";
        public string Subcommands { get; set; } = "Subcommands";
        public string Usage { get; set; } = "Usage";

        [Description("Translation for command responses.")]
        public string NoPermissions { get; set; } = "You don't have permission to use that command.";
        public string NoPlayers { get; set; } = "Provided player(s) doesn't exist.";
        public string RoundNotStarted { get; set; } = "You can't use that command before round start.";
        public string SenderNull { get; set; } = "Commandsender is null.";

        internal static Translation AccessTranslation()
        {
            return translation ??= File.Exists(filePath) ? YamlParser.Deserializer.Deserialize<Translation>(File.ReadAllText(filePath)) : new();
        }

        private static Translation translation;

        private static readonly string filePath = Path.Combine(PathManager.Configs.FullName, Server.Port.ToString(), "Scp008", "translation.yml");
    }
}
