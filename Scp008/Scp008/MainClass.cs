using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Log = LabApi.Features.Console.Logger;

using HarmonyLib;
using LabApi.Events.CustomHandlers;
using LabApi.Features;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;

namespace Scp008
{
    public class MainClass : Plugin<Config>
    {
        public override void LoadConfigs()
        {
            pluginTranslation = this.LoadConfig<Translation>("translation.yml");
            if (string.IsNullOrWhiteSpace(pluginTranslation.InfectionDeathReason))
            {
                pluginTranslation.InfectionDeathReason = "Killed by a mysterious infection.";
                Log.Warn("Property \"infection_death_reason\" cannot be null in translation file, added default translation.");
            }
            base.LoadConfigs();
            pluginConfig = Config;
        }

        public override void Enable()
        {
            Instance = this;
            Events = new();
            CustomHandlersManager.RegisterEventsHandler(Events);
            harmony = new($"{Name.ToLower()}.{DateTime.UtcNow.Ticks}");
            harmony.PatchAll();
        }

        public override void Disable()
        {
            harmony.UnpatchAll();
            harmony = null;
            CustomHandlersManager.UnregisterEventsHandler(Events);
            Events = null;
            pluginConfig = null;
            Instance = null;
        }

        public Config pluginConfig;
        public Translation pluginTranslation;
        private Harmony harmony;

        public EventHandler Events { get; private set; }
        public static MainClass Instance { get; private set; }

        public override string Author { get; } = "Catiatto";
        public override string Description { get; } = null;
        public override string Name { get; } = "Scp008";
        public override Version RequiredApiVersion { get; } = new(LabApiProperties.CompiledVersion);
        public override Version Version { get; } = new(2, 0, 7);
    }
}

