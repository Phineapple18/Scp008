using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomPlayerEffects;
using PluginAPI.Core;
using UnityEngine;

namespace Scp008
{
    [DisallowMultipleComponent]
    public class Scp008Component : MonoBehaviour
    {
        public void Awake()
        {
            player = Player.Get(ReferenceHub.GetHub(base.transform.root.gameObject));
            Log.Debug($"Created a {this.GetType().Name} for player {player.Nickname}.", config.Debug, pluginName);
        }

        public void OnEnable()
        {
            player.TemporaryData.Add(Scp008Extensions.dataName, "spawned");
            intervalTime = Plugin.Singleton.pluginConfig.InfectionInterval;
            Log.Debug($"Enabled a {this.GetType().Name} for player {player.Nickname}.", config.Debug, pluginName);
        }

        public void Update()
        {
            foreach (var effect in config.Scp008Effects)
            {
                if (player.Health < effect.Value && player.EffectsManager.TryGetEffect(effect.Key, out StatusEffectBase effectBase))
                {
                    effectBase.ServerSetState(Scp008Extensions.intensity);
                }
            }
            intervalTime -= Time.deltaTime;
            if (intervalTime <= 0)
            {
                player.Damage(config.InfectionDamage, translation.InfectionDeathReason);
                string message = translation.InfectionMessages.LastOrDefault(m => player.Health <= m.Value).Key;
                if (!receivedHints.Contains(message))
                {
                    player.ReceiveHint(message, 5);
                    receivedHints.Add(message);
                }
                intervalTime += config.InfectionInterval;
            }
        }

        public void OnDisable()
        {
            receivedHints.Clear();
            player.EffectsManager.DisableAllEffects();
            player.TemporaryData.Remove(Scp008Extensions.dataName);
            Log.Debug($"Disabled a {this.GetType().Name} for player {player.Nickname}.", config.Debug, pluginName);
        }

        private Player player;

        private float intervalTime;

        private readonly List<string> receivedHints = new();

        private readonly Config config = Plugin.Singleton.pluginConfig;

        private readonly Translation translation = Plugin.Singleton.pluginTranslation;

        private readonly string pluginName = Plugin.Singleton.pluginHandler.PluginName;
    }
}
