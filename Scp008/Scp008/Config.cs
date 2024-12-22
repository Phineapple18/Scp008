using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

namespace Scp008
{
    public class Config
    {
        [Description("Should the plugin be enabled?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Should debug be enabled?")]
        public bool Debug { get; set; } = false;

        [Description("Infection chance per Zombie attack. Set between 0-100.")]
        public int InfectionChance { get; set; } = 50;

        [Description("Damage per infection tick.")]
        public float InfectionDamage { get; set; } = 5f;

        [Description("Interval between infection ticks.")]
        public float InfectionInterval { get; set; } = 5f;

        [Description("Damage of a Zombie attack leading to infection. Set below 0 to leave unchanged.")]
        public float ZombieDamage { get; set; } = 5f;

        [Description("Effect(s), that will be enabled once player health drops below certain values.")]
        public Dictionary<string, float> Scp008Effects { get; set; } = new()
        {
            { "Bleeding", 90},
            { "Concussed", 50},
            { "Deafened", 20}
        };

        [Description("Item(s), that can cure the infection and their cure chance. Set between 0-100.")]
        public Dictionary<ItemType, int> CureItems { get; set; } = new()
        {
            { ItemType.SCP500, 100},
            { ItemType.Medkit, 50}
        };   

        [Description("Health threshold of an infected player, below which they can be killed by a player from the same faction (only on servers with Firendly Fire disabled).")]
        public float FfHealthCutoff { get; set; } = 20f;

        [Description("Cause(s) of death, that will cause an infected player to turn into a Zombie upon death. Leave empty to disable.")]
        public List<string> DeathReasons { get; set; } = new()
        {
            "Infection",
            "Scp0492",
            "Scp049",
            "ZombieFlamingo",
            "Any"
        };

        [Description("Can Flamingos (except Alpha Flamingo) be infected with SCP-008?")]
        public bool CanFlamingoBeInfected { get; set; } = true;

        [Description("Can Zombie Flamingos infect with SCP-008?")]
        public bool CanFlamingoInfect { get; set; } = false;
    }
}
