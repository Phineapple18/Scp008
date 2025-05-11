using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomPlayerEffects;
using InventorySystem;
using InventorySystem.Items;
using Scp1344Item = InventorySystem.Items.Usables.Scp1344.Scp1344Item;
using Log = LabApi.Features.Console.Logger;
using LabApi.Features.Wrappers;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace Scp008.Features
{
    [DisallowMultipleComponent]
    public class Scp008Component : MonoBehaviour
    {
        public void Awake()
        {
            player = Player.Get(transform.root.gameObject);
            Log.Debug($"Created a Scp008Component for player {player.Nickname}.", config.Debug);
        }

        public void OnEnable()
        {
            intervalTime = MainClass.Instance.pluginConfig.InfectionInterval;
            Log.Debug($"Enabled a Scp008Component for player {player.Nickname}.", config.Debug);
        }

        public void Update()
        {
            config.Scp008Effects?.ForEach(effect =>
            {
                EffectParameters parameters = effect.Value.OrderByDescending(e => e.Health).LastOrDefault(e => player.Health < e.Health);
                if (parameters != null && player.TryGetEffect(effect.Key, out StatusEffectBase effectBase) && effectBase.Intensity != parameters.Intensity)
                {
                    if (effectBase is Blindness
                    && (player.ActiveEffects.Any(e => e is SeveredEyes)
                    || player.Inventory.TryGetInventoryItem(ItemType.SCP1344, out ItemBase item)
                    && Enumerable.Range(2, 8).Contains((byte)(item as Scp1344Item).Status)))
                    {
                        return;
                    }
                    player.EnableEffect(effectBase, parameters.Intensity);
                }
            });
            intervalTime -= Time.deltaTime;
            if (intervalTime <= 0)
            {
                player.Damage(config.InfectionDamage, translation.InfectionDeathReason);
                if (player.IsHuman)
                {
                    string message = translation.InfectionMessages.LastOrDefault(m => player.Health <= m.Key).Value;
                    if (receivedHints.AddIfNotContains(message))
                    {
                        player.SendHint(message, 5);
                    }
                }
                intervalTime += config.InfectionInterval;
            }
        }

        public void OnDisable()
        {
            receivedHints.Clear();
            player.DisableAllEffects();
            Log.Debug($"Disabled a {this.GetType().Name} for player {player.Nickname}.", config.Debug);
        }

        private Player player;
        private float intervalTime;
        private readonly List<string> receivedHints = new();

        private readonly Config config = MainClass.Instance.pluginConfig;
        private readonly Translation translation = MainClass.Instance.pluginTranslation;
    }
}
