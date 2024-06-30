using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using HarmonyLib;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using PluginAPI.Helpers;
using PluginAPI.Loader;

namespace Scp008
{
    public class Plugin
    {
        [PluginPriority(LoadPriority.Medium)]
        [PluginEntryPoint(Translation.pluginName, "1.0.0", null, "Phineapple18")]
        public void OnLoad()
        {
            if (!pluginConfig.IsEnabled)
            {
                Log.Warning($"Plugin {Translation.pluginName} is disabled.", Translation.pluginName);
                return;
            }
            if (!AssemblyLoader.InstalledPlugins.Any(p => p.PluginName == "NWApiPermissionSystem"))
            {
                throw new DllNotFoundException($"The NWApiPermissionSystem plugin is required for {Translation.pluginName} plugin to work.");
            }
            if (File.Exists(Path.Combine(Paths.GlobalPlugins.Plugins, "0Harmony.dll")) || File.Exists(Path.Combine(Paths.LocalPlugins.Plugins, "0Harmony.dll")))
            {
                Log.Warning("0Harmony should be in the dependencies folder.", Translation.pluginName);
            }
            Singleton = this;
            pluginHandler = PluginHandler.Get(this);
            EventManager.RegisterEvents<EventHandlers>(this);
            this.harmony = new Harmony($"{pluginHandler.PluginName.ToLower()}.{DateTime.UtcNow.Ticks}");
            this.harmony.PatchAll();
            Log.Info($"Loaded plugin {pluginHandler.PluginName} by {pluginHandler.PluginAuthor}.", pluginHandler.PluginName);
        }

        private Harmony harmony;

        public PluginHandler pluginHandler;

        [PluginConfig] public Config pluginConfig;

        [PluginConfig(Translation.translationFileName)] public Translation pluginTranslation;

        public static Plugin Singleton { get; private set; }
    }
}
