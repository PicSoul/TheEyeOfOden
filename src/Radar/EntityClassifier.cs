using UnityEngine;

namespace TheEyeOfOden.Radar
{
    public struct EntityClassification
    {
        public bool Display;
        public EntityCategory Category;
        public Color Color;
        public float Scale;
        public int Stars;
        public bool IsAlerted;
        public bool IsBoss;
        public float ElevationDiff;
        public string Name;
    }

    public static class EntityClassifier
    {
        public static EntityClassification Classify(Character character, Player localPlayer)
        {
            EntityClassification result = default;
            if (character == null || character.IsDead() || character == localPlayer)
            {
                return result;
            }

            Vector3 playerPos = localPlayer.transform.position;
            Vector3 charPos = character.transform.position;

            // Distance filter
            float dist = Vector3.Distance(playerPos, charPos);
            float maxRange = ModConfig.RadarRange.Value;
            if (maxRange > 0f && dist > maxRange)
            {
                return result;
            }

            int stars = Mathf.Max(0, character.GetLevel() - 1);
            bool isBoss = character.IsBoss() || character.m_boss || character.GetFaction() == Character.Faction.Boss;
            bool isTamed = character.IsTamed() || character.GetFaction() == Character.Faction.PlayerSpawned;
            bool isPlayer = character.IsPlayer();
            bool isAlerted = character.GetBaseAI()?.IsAlerted() == true;

            EntityCategory category = EntityCategory.Unknown;
            Color color = Color.white;

            if (isPlayer)
            {
                if (!ModConfig.ShowOtherPlayers.Value) return result;
                category = EntityCategory.Player;
                color = ModConfig.PlayerColor.Value;
            }
            else if (isBoss)
            {
                if (!ModConfig.ShowBosses.Value) return result;
                category = EntityCategory.Boss;
                color = ModConfig.BossColor.Value;
            }
            else if (isTamed)
            {
                if (!ModConfig.ShowTamed.Value) return result;
                category = EntityCategory.Tamed;
                color = ModConfig.TamedColor.Value;
            }
            else if (character.GetFaction() == Character.Faction.AnimalsVeg)
            {
                if (isAlerted)
                {
                    if (!ModConfig.ShowHostiles.Value) return result;
                    category = EntityCategory.AlertedHostile;
                    color = ModConfig.AlertedColor.Value;
                }
                else
                {
                    if (!ModConfig.ShowPassives.Value) return result;
                    category = EntityCategory.Passive;
                    color = ModConfig.PassiveColor.Value;
                }
            }
            else if (character.GetFaction() == Character.Faction.Dverger)
            {
                if (isAlerted)
                {
                    if (!ModConfig.ShowHostiles.Value) return result;
                    category = EntityCategory.AlertedHostile;
                    color = ModConfig.AlertedColor.Value;
                }
                else
                {
                    if (!ModConfig.ShowNpcs.Value) return result;
                    category = EntityCategory.Npc;
                    color = ModConfig.NpcColor.Value;
                }
            }
            else
            {
                // Hostile monsters
                if (isAlerted)
                {
                    if (!ModConfig.ShowAlerted.Value && !ModConfig.ShowHostiles.Value) return result;
                    category = EntityCategory.AlertedHostile;
                    color = ModConfig.AlertedColor.Value;
                }
                else
                {
                    if (!ModConfig.ShowHostiles.Value) return result;
                    category = EntityCategory.Hostile;
                    color = ModConfig.HostileColor.Value;
                }
            }

            // Starred filter
            if (ModConfig.ShowStarredOnly.Value && stars < 1 && !isBoss && !isPlayer)
            {
                return result;
            }

            float scale = 1f;
            if (isBoss)
            {
                scale *= ModConfig.BossScale.Value;
            }
            else if (stars >= 1)
            {
                scale *= ModConfig.StarredScale.Value;
            }

            float elevationDiff = charPos.y - playerPos.y;

            result.Display = true;
            result.Category = category;
            result.Color = color;
            result.Scale = scale;
            result.Stars = stars;
            result.IsAlerted = isAlerted;
            result.IsBoss = isBoss;
            result.ElevationDiff = elevationDiff;
            result.Name = character.GetHoverName();

            return result;
        }
    }
}
