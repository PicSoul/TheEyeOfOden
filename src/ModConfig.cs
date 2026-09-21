using BepInEx.Configuration;
using UnityEngine;
using TheEyeOfOden.Radar;

namespace TheEyeOfOden
{
    public static class ModConfig
    {
        // 1. General
        public static ConfigEntry<bool> ModEnabled { get; private set; }
        public static ConfigEntry<KeyboardShortcut> ToggleKey { get; private set; }
        public static ConfigEntry<float> RadarRange { get; private set; }
        public static ConfigEntry<bool> ShowOnSmallMap { get; private set; }
        public static ConfigEntry<bool> ShowOnLargeMap { get; private set; }
        public static ConfigEntry<bool> ClampToMinimapEdge { get; private set; }

        // 2. Filters
        public static ConfigEntry<bool> ShowHostiles { get; private set; }
        public static ConfigEntry<bool> ShowAlerted { get; private set; }
        public static ConfigEntry<bool> ShowPassives { get; private set; }
        public static ConfigEntry<bool> ShowTamed { get; private set; }
        public static ConfigEntry<bool> ShowBosses { get; private set; }
        public static ConfigEntry<bool> ShowOtherPlayers { get; private set; }
        public static ConfigEntry<bool> ShowNpcs { get; private set; }
        public static ConfigEntry<bool> ShowStarredOnly { get; private set; }

        // 3. Visuals
        public static ConfigEntry<DisplayMode> RadarDisplayMode { get; private set; }
        public static ConfigEntry<float> DotSize { get; private set; }
        public static ConfigEntry<float> BossScale { get; private set; }
        public static ConfigEntry<float> StarredScale { get; private set; }
        public static ConfigEntry<bool> AlertedPulse { get; private set; }
        public static ConfigEntry<bool> ShowElevationIndicators { get; private set; }
        public static ConfigEntry<float> ElevationThreshold { get; private set; }

        // 4. Colors
        public static ConfigEntry<Color> HostileColor { get; private set; }
        public static ConfigEntry<Color> AlertedColor { get; private set; }
        public static ConfigEntry<Color> PassiveColor { get; private set; }
        public static ConfigEntry<Color> TamedColor { get; private set; }
        public static ConfigEntry<Color> BossColor { get; private set; }
        public static ConfigEntry<Color> PlayerColor { get; private set; }
        public static ConfigEntry<Color> NpcColor { get; private set; }

        public static void Bind(ConfigFile config)
        {
            // General
            ModEnabled = config.Bind(
                "1. General",
                "Enabled",
                true,
                "Master switch for The Eye of Oden radar.");

            ToggleKey = config.Bind(
                "1. General",
                "ToggleKey",
                new KeyboardShortcut(KeyCode.None),
                "Optional hotkey to toggle the radar on and off in-game.");

            RadarRange = config.Bind(
                "1. General",
                "RadarRange",
                120f,
                new ConfigDescription("Maximum detection radius around the player in meters. Set to 0 for unlimited loaded zone range.",
                    new AcceptableValueRange<float>(0f, 500f)));

            ShowOnSmallMap = config.Bind(
                "1. General",
                "ShowOnSmallMap",
                true,
                "Show radar entities on the HUD minimap (top right).");

            ShowOnLargeMap = config.Bind(
                "1. General",
                "ShowOnLargeMap",
                true,
                "Show radar entities on the fullscreen map (M key).");

            ClampToMinimapEdge = config.Bind(
                "1. General",
                "ClampToMinimapEdge",
                false,
                "If true, entities beyond the small minimap edge are clamped to the border like a compass indicator.");

            // Filters
            ShowHostiles = config.Bind(
                "2. Filters",
                "ShowHostiles",
                true,
                "Display hostile monsters on the radar.");

            ShowAlerted = config.Bind(
                "2. Filters",
                "ShowAlerted",
                true,
                "Display alerted/aggressive enemies on the radar.");

            ShowPassives = config.Bind(
                "2. Filters",
                "ShowPassives",
                true,
                "Display passive wildlife (Deer, Birds, Fish, Hares, etc.) on the radar.");

            ShowTamed = config.Bind(
                "2. Filters",
                "ShowTamed",
                true,
                "Display tamed creatures and player summons on the radar.");

            ShowBosses = config.Bind(
                "2. Filters",
                "ShowBosses",
                true,
                "Display boss creatures on the radar.");

            ShowOtherPlayers = config.Bind(
                "2. Filters",
                "ShowOtherPlayers",
                true,
                "Display other players in multiplayer on the radar.");

            ShowNpcs = config.Bind(
                "2. Filters",
                "ShowNpcs",
                true,
                "Display friendly/neutral NPCs (Dverger, Merchants) on the radar.");

            ShowStarredOnly = config.Bind(
                "2. Filters",
                "ShowStarredOnly",
                false,
                "If true, only 1-star, 2-star, and boss creatures are displayed.");

            // Visuals
            RadarDisplayMode = config.Bind(
                "3. Visuals",
                "DisplayMode",
                DisplayMode.DotsOnly,
                "Display style: DotsOnly (default colored dots), IconsWithDotsFallback, or IconsOnly.");

            DotSize = config.Bind(
                "3. Visuals",
                "DotSize",
                8f,
                new ConfigDescription("Base diameter of radar dots in pixels.",
                    new AcceptableValueRange<float>(4f, 32f)));

            BossScale = config.Bind(
                "3. Visuals",
                "BossScale",
                1.6f,
                new ConfigDescription("Scale multiplier for boss markers.",
                    new AcceptableValueRange<float>(1f, 3f)));

            StarredScale = config.Bind(
                "3. Visuals",
                "StarredScale",
                1.3f,
                new ConfigDescription("Scale multiplier for 1-star and 2-star creature markers.",
                    new AcceptableValueRange<float>(1f, 2.5f)));

            AlertedPulse = config.Bind(
                "3. Visuals",
                "AlertedPulse",
                true,
                "Pulsing animation for alerted/hostile creatures.");

            ShowElevationIndicators = config.Bind(
                "3. Visuals",
                "ShowElevationIndicators",
                true,
                "Show subtle elevation chevrons when an entity is significantly above or below the player.");

            ElevationThreshold = config.Bind(
                "3. Visuals",
                "ElevationThreshold",
                5f,
                new ConfigDescription("Height difference in meters to trigger the elevation chevron.",
                    new AcceptableValueRange<float>(1f, 25f)));

            // Colors
            HostileColor = config.Bind(
                "4. Colors",
                "HostileColor",
                new Color(1f, 0.3f, 0.3f, 1f),
                "Color for unalerted hostile creatures (Coral Red).");

            AlertedColor = config.Bind(
                "4. Colors",
                "AlertedColor",
                new Color(1f, 0.1f, 0.1f, 1f),
                "Color for alerted / in-combat hostile creatures (Vivid Red).");

            PassiveColor = config.Bind(
                "4. Colors",
                "PassiveColor",
                new Color(0.33f, 1f, 0.33f, 1f),
                "Color for passive wildlife (Soft Green).");

            TamedColor = config.Bind(
                "4. Colors",
                "TamedColor",
                new Color(0f, 1f, 0.55f, 1f),
                "Color for tamed creatures (Spring Green).");

            BossColor = config.Bind(
                "4. Colors",
                "BossColor",
                new Color(1f, 0f, 1f, 1f),
                "Color for boss creatures (Magenta).");

            PlayerColor = config.Bind(
                "4. Colors",
                "PlayerColor",
                new Color(0f, 0.8f, 1f, 1f),
                "Color for other players (Cyan).");

            NpcColor = config.Bind(
                "4. Colors",
                "NpcColor",
                new Color(1f, 0.84f, 0f, 1f),
                "Color for neutral NPCs and merchants (Gold).");
        }
    }
}
