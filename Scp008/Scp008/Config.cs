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
        [Description("Should debug be enabled?")]
        public bool Debug { get; set; } = false;

        [Description("Infection chance from Zombie attack depending on their count. The exact number or highest less than will be used.")]
        public Dictionary<int, int> InfectionChance { get; set; } = new()
        {
            { 1, 30 },
            { 5, 50 }
        };
        
        [Description("Infection damage per tick.")]
        public float InfectionDamage { get; set; } = 5f;
        
        [Description("Interval between infection ticks.")]
        public float InfectionInterval { get; set; } = 5f;
        
        [Description("Should zombies only be able to infect when there is no SCP-049?")]
        public bool InfectNo049Only { get; set; } = false;
        
        [Description("Zombie attack damage upon infection. Set to 0 or below to leave unchanged.")]
        public float ZombieDamage { get; set; } = 5f;

        [Description("Effects, that will be enabled with specific intensity once player's health drops below certain threshold.")]
        public Dictionary<string, List<EffectParameters>> Scp008Effects { get; set; } = new()
        {
            { 
                "Bleeding", new()
                { 
                    new() { Health = 90f, Intensity = 1 }
                } 
            },
            { 
                "Blindness", new() 
                { 
                    new() { Health = 60f, Intensity = 10 },
                    new() { Health = 30f, Intensity = 20 } 
                } 
            }
        };

        [Description("Items able to cure infection and their cure chance. Set between 0-100.")]
        public Dictionary<ItemType, int> CureItems { get; set; } = new()
        {
            { ItemType.SCP500, 100},
            { ItemType.Medkit, 50}
        };

        [Description("Health threshold of infected player below which they can be attacked by their factionmates with no conseqences.")]
        public float FriendlyfireHealth { get; set; } = 20f;

        [Description("Death causes that will turn infected player into a Zombie upon death. Leave empty to disable.")]
        public List<string> DeathReasons { get; set; } = new()
        {
            "Infection",
            "Scp0492",
            "Scp049",
            "ZombieFlamingo",
            "Explosion",
            "MicroHID",
            "Any"
        };

        [Description("Can Flamingos (except Alpha) be infected with SCP-008?")]
        public bool FlamingoInfected { get; set; } = true;

        [Description("Can Zombie Flamingos infect with SCP-008?")]
        public bool FlamingoInfect { get; set; } = false;
    }

    public class EffectParameters
    {
        public float Health { get; set; }
        public byte Intensity { get; set; }
    }
}
