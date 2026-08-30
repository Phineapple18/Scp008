using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomPlayerEffects;
using InventorySystem;
using InventorySystem.Items;
using LabApi.Features.Wrappers;
using UnityEngine;
using Utils.NonAllocLINQ;
using Log = LabApi.Features.Console.Logger;
using Scp1344Item = InventorySystem.Items.Usables.Scp1344.Scp1344Item;

namespace Scp008.Features
{
    [DisallowMultipleComponent]
    public class Scp008Component : MonoBehaviour
    {
        public void Awake()
        {
            player = Player.Get(transform.root.gameObject);
            Log.Debug($"Created a Scp008Component for player {player.Nickname}.", Config.Debug);
        }

        public void OnEnable()
        {
            intervalTime = Config.InfectionInterval;
            player.CustomInfo = Translation.InfectedCustomInfo;
            Log.Debug($"Enabled a Scp008Component for player {player.Nickname}.", Config.Debug);
        }

        public void Update()
        {
            Config.Scp008Effects?.ForEach(effect =>
            {
                if (!allowedEffects.Contains(effect.Key))
                {
                    return;
                }
                EffectParameters parameters = effect.Value.OrderByDescending(e => e.Health).LastOrDefault(e => player.Health < e.Health);
                if (parameters != null && player.TryGetEffect(effect.Key, out StatusEffectBase effectBase) && effectBase.Intensity != parameters.Intensity)
                {
                    if (effectBase is Blindness && (player.HasEffect<SeveredEyes>()
                    || player.Inventory.TryGetInventoryItem(ItemType.SCP1344, out ItemBase item)
                    && Enumerable.Range(2, 8).Contains((byte)(item as Scp1344Item).Status)))
                    {
                        return;
                    }
                    player.EnableEffect(effectBase, parameters.Intensity, intervalTime + 1f);
                }
            });
            intervalTime -= Time.deltaTime;
            if (intervalTime <= 0)
            {
                player.Damage(Config.InfectionDamage, Translation.InfectionDeathReason);
                if (player.IsHuman)
                {
                    string message = Translation.InfectionMessages?.LastOrDefault(m => player.Health <= m.Key).Value;
                    if (message != null && receivedHints.AddIfNotContains(message))
                    {
                        player.SendHint(message, 5);
                    }
                }
                intervalTime += Config.InfectionInterval;
            }
        }

        public void OnDisable()
        {
            player.CustomInfo = null;
            receivedHints.Clear();
            player.DisableAllEffects();
            Log.Debug($"Disabled a {this.GetType().Name} for player {player.Nickname}.", Config.Debug);
        }

        private Player player;
        private float intervalTime;
        private readonly List<string> receivedHints = new();
        private static readonly List<string> allowedEffects = new()
        {
            "AmnesiaItems",
            "Bleeding",
            "Blindness",
            "Blurriness",
            "Burned",
            "CardiacArrest",
            "Concussed",
            "Deafened",
            "Disabled",
            "Exhausted",
            "FogControl",
            "Hemorrhage",
            "Lightweight",
            "NightVision",
            "HeavyFooted",
            "MovementBoost",
            "Poisoned",
            "Slowness"
        };

        private Config Config => MainClass.Instance.pluginConfig;
        private Translation Translation => MainClass.Instance.pluginTranslation;
    }
}
