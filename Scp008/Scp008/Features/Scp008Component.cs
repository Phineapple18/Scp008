using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomPlayerEffects;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Usables.Scp1344;
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
            player = Player.Get(ReferenceHub.GetHub(transform.root.gameObject));
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
                if (parameters != null && player.ReferenceHub.playerEffectsController.TryGetEffect(effect.Key, out StatusEffectBase effectBase) && effectBase.Intensity != parameters.Intensity)
                {
                    if (effect.Key == nameof(Blindness) 
                    && (player.ActiveEffects.ToList().Any(e => e.name == nameof(SeveredEyes))
                    || player.ReferenceHub.inventory.TryGetInventoryItem(ItemType.SCP1344, out ItemBase item)
                    && (byte)(item as Scp1344Item).Status > 2 && (byte)(item as Scp1344Item).Status < 8))
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
                string message = translation.InfectionMessages.LastOrDefault(m => player.Health <= m.Key).Value;
                if (player.IsHuman && !receivedHints.Contains(message))
                {
                    player.SendHint(message, 5);
                    receivedHints.Add(message);
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
