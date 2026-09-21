using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using TheEyeofOden.Radar;

namespace TheEyeofOden
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class EyeOfOdenPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.pics0ul.valheim.theeyeofoden";
        public const string PluginName = "The Eye of Oden";
        public const string PluginVersion = "0.1.0";

        internal static ManualLogSource Log { get; private set; }
        private Harmony _harmony;

        internal static readonly List<string> FailedPatches = new List<string>();

        private void Awake()
        {
            Log = Logger;

            ModConfig.Bind(Config);

            string pluginDir = Path.GetDirectoryName(Info.Location);
            EntityIconResolver.Initialize(pluginDir);

            _harmony = new Harmony(PluginGuid);
            ApplyPatches();

            // In case Minimap is already active (e.g. late load or script reload)
            if (Minimap.instance != null)
            {
                RadarOverlay.Initialize(Minimap.instance);
            }

            LogActiveConfiguration();
        }

        private void ApplyPatches()
        {
            int applied = 0;
            int failed = 0;

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(HarmonyPatch), inherit: true).Length == 0)
                {
                    continue;
                }

                try
                {
                    _harmony.CreateClassProcessor(type).Patch();
                    applied++;
                }
                catch (Exception ex)
                {
                    failed++;
                    FailedPatches.Add(type.Name);
                    Log.LogError($"Patch '{type.Name}' could not be applied: {ex.Message}");
                }
            }

            if (failed == 0)
            {
                Log.LogInfo($"{PluginName} {PluginVersion} loaded; {applied} patch classes applied.");
            }
            else
            {
                Log.LogWarning($"{PluginName} {PluginVersion} loaded with {applied} of {applied + failed} patches applied.");
            }
        }

        private void LogActiveConfiguration()
        {
            Log.LogInfo($"  enabled: {ModConfig.ModEnabled.Value}, range: {ModConfig.RadarRange.Value}m");
            Log.LogInfo($"  small map: {ModConfig.ShowOnSmallMap.Value}, large map: {ModConfig.ShowOnLargeMap.Value}");
            Log.LogInfo($"  display mode: {ModConfig.RadarDisplayMode.Value}, dot size: {ModConfig.DotSize.Value}px");
            Log.LogInfo($"  filters: hostiles={ModConfig.ShowHostiles.Value}, passives={ModConfig.ShowPassives.Value}, tamed={ModConfig.ShowTamed.Value}, bosses={ModConfig.ShowBosses.Value}, players={ModConfig.ShowOtherPlayers.Value}");
        }

        private void Update()
        {
            KeyboardShortcut toggleKey = ModConfig.ToggleKey.Value;
            if (toggleKey.MainKey != UnityEngine.KeyCode.None && toggleKey.IsDown())
            {
                ModConfig.ModEnabled.Value = !ModConfig.ModEnabled.Value;
                string status = ModConfig.ModEnabled.Value ? "Enabled" : "Disabled";
                Log.LogInfo($"Radar {status}");

                if (Player.m_localPlayer != null && MessageHud.instance != null)
                {
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, $"The Eye of Oden: {status}");
                }
            }
        }

        private void OnDestroy()
        {
            try
            {
                _harmony?.UnpatchSelf();
            }
            catch { }

            _harmony = null;
            RadarOverlay.Destroy();
        }
    }
}
