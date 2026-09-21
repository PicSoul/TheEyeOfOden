using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TheEyeOfOden.Radar
{
    public static class EntityIconResolver
    {
        private static readonly Dictionary<string, Sprite> IconCache = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);
        private static readonly HashSet<string> MissCache = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private static string _customIconsDirectory;

        public static void Initialize(string pluginDirectory)
        {
            _customIconsDirectory = Path.Combine(pluginDirectory, "icons");
            if (!Directory.Exists(_customIconsDirectory))
            {
                try
                {
                    Directory.CreateDirectory(_customIconsDirectory);
                }
                catch { }
            }
        }

        /// <summary>
        /// Attempt to resolve an entity icon sprite for a Character.
        /// Returns true if an icon is resolved, false if it should fall back to a colored dot.
        /// </summary>
        public static bool TryGetIcon(Character character, out Sprite icon)
        {
            icon = null;

            if (ModConfig.RadarDisplayMode.Value == DisplayMode.DotsOnly)
            {
                return false;
            }

            if (character == null) return false;

            string rawName = character.gameObject.name;
            string cleanName = rawName.Replace("(Clone)", "").Trim();

            // Check cache
            if (IconCache.TryGetValue(cleanName, out icon))
            {
                return icon != null;
            }

            if (MissCache.Contains(cleanName))
            {
                return false;
            }

            // 1. Check custom icons on disk
            icon = LoadCustomDiskIcon(cleanName);
            if (icon != null)
            {
                IconCache[cleanName] = icon;
                return true;
            }

            // 2. Query ObjectDB for matching Valheim trophy icon
            icon = FindGameTrophyIcon(cleanName);
            if (icon != null)
            {
                IconCache[cleanName] = icon;
                return true;
            }

            // Not found, record miss to avoid repeated searches
            MissCache.Add(cleanName);
            return false;
        }

        private static Sprite LoadCustomDiskIcon(string prefabName)
        {
            if (string.IsNullOrEmpty(_customIconsDirectory) || !Directory.Exists(_customIconsDirectory))
            {
                return null;
            }

            string filePath = Path.Combine(_customIconsDirectory, prefabName + ".png");
            if (!File.Exists(filePath)) return null;

            try
            {
                byte[] bytes = File.ReadAllBytes(filePath);
                Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                if (ImageConversion.LoadImage(tex, bytes))
                {
                    tex.name = "CustomIcon_" + prefabName;
                    return Sprite.Create(
                        tex,
                        new Rect(0f, 0f, tex.width, tex.height),
                        new Vector2(0.5f, 0.5f),
                        100f);
                }
            }
            catch
            {
                // Ignore load failures
            }

            return null;
        }

        private static Sprite FindGameTrophyIcon(string prefabName)
        {
            if (ObjectDB.instance == null) return null;

            // Common trophy name patterns in Valheim: "Trophy<PrefabName>"
            string[] candidateTrophyNames = new string[]
            {
                "Trophy" + prefabName,
                "Trophy" + prefabName.Replace("Character", ""),
                "Trophy" + MapPrefabToTrophySuffix(prefabName)
            };

            foreach (string trophyName in candidateTrophyNames)
            {
                if (string.IsNullOrEmpty(trophyName)) continue;

                GameObject itemPrefab = ObjectDB.instance.GetItemPrefab(trophyName);
                if (itemPrefab != null)
                {
                    ItemDrop itemDrop = itemPrefab.GetComponent<ItemDrop>();
                    if (itemDrop != null && itemDrop.m_itemData?.m_shared?.m_icons != null && itemDrop.m_itemData.m_shared.m_icons.Length > 0)
                    {
                        Sprite s = itemDrop.m_itemData.m_shared.m_icons[0];
                        if (s != null) return s;
                    }
                }
            }

            return null;
        }

        private static string MapPrefabToTrophySuffix(string prefabName)
        {
            // Valheim internal prefab names that differ slightly from their trophy item names
            switch (prefabName)
            {
                case "Goblin": return "Goblin";
                case "GoblinShaman": return "GoblinShaman";
                case "GoblinBrute": return "GoblinBrute";
                case "GoblinKing": return "GoblinKing";
                case "DragonQueen": return "DragonQueen";
                case "Greydwarf_Elite": return "GreydwarfBrute";
                case "Greydwarf_Shaman": return "GreydwarfShaman";
                default: return prefabName;
            }
        }
    }
}
