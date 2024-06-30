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
        [Description("Should plugin be enabled?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Should debug be enabled?")]
        public bool Debug { get; set; } = false;

        [Description("Damage, that a zombie attack will deal, if it leads to infection. Set below 0 to disable modifying infection damage.")]
        public float ZombieDamage { get; set; } = 5f;

        [Description("Infection chance. Set between 0-100.")]
        public int InfectionChance { get; set; } = 50;

        [Description("Damage per infection tick.")]
        public float InfectionDamage { get; set; } = 5f;

        [Description("Interval between infection damage ticks.")]
        public float InfectionInterval { get; set; } = 5f;

        [Description("Effects, that will be enabled, if player health drops below set values.")]
        public Dictionary<string, float> Scp008Effects { get; set; } = new()
        {
            { "Bleeding", 90},
            { "Concussed", 50},
            { "Deafened", 20}
        };

        [Description("Items, that can cure the infection and the cure chance. Set between 0-100.")]
        public Dictionary<ItemType, int> CureItems { get; set; } = new()
        {
            { ItemType.SCP500, 100},
            { ItemType.Medkit, 50}
        };   

        [Description("Amount of HP of infected player, below which they can be killed by a player from the same faction, if Firendly Fire is disabled.")]
        public float FfHealthCutoff { get; set; } = 20f;  

        [Description("Causes of death, that will lead an infected player to become SCP-0492 upon death. Leave empty to disable.")]
        public List<string> TurnIntoZombie { get; set; } = new()
        {
            "Infection",
            "Scp0492",
            "Scp049",
            "Any"
        };
    }
}
