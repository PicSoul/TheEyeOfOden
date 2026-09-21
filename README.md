# The Eye of Oden

**The Eye of Oden** is a high-performance entity radar mod for **Valheim**. It displays nearby creatures, monsters, bosses, tamed animals, and players as clean, anti-aliased colored dots on both the HUD minimap and the fullscreen map.

Designed with zero-allocation pooling to ensure buttery-smooth performance with no garbage collection stutter even in dense creature spawns.

---

## Features

- **Smooth Anti-Aliased Colored Dots**: Procedurally rendered at runtime; crisp at any resolution without requiring external asset bundles.
- **Categorized Color-Coding**:
  - **Red (`#FF4D4D`)**: Hostile monsters (idle/patrolling).
  - **Bright Red (`#FF1A1A`) & Pulsing Ring**: Alerted / in-combat enemies.
  - **Soft Green (`#55FF55`)**: Passive wildlife (Deer, Birds, Fish, Hares).
  - **Spring Green (`#00FF88`)**: Tamed creatures and player summons.
  - **Magenta (`#FF00FF`) & Ring**: Bosses and mini-bosses (larger scale).
  - **Cyan (`#00D0FF`)**: Other players in multiplayer.
  - **Gold (`#FFD700`)**: Neutral NPCs and merchants (Dverger, Haldor, Hildir, Bog Witch).
- **Elevation Indicators**: Subtle upward or downward chevrons appear on dots when entities are significantly above or below the player (ideal for mountains, crypts, and steep terrain).
- **Star Level Scaling**: 1-star and 2-star creatures are scaled up slightly for quick visual identification.
- **Dual Map Support**: Renders seamlessly on both the small HUD minimap and the fullscreen map (`M`).
- **Zero GC Allocations**: Uses a custom UI pool (`RadarMarkerPool`) that recycles marker objects every frame.
- **Future Icon Support**: Built-in `EntityIconResolver` designed to automatically resolve trophy icons or custom sprite packs when `DisplayMode` is enabled.
- **Fully Configurable**: Toggle individual entity filters, adjust detection range, change dot sizes, customize colors, or bind a master toggle key.

---

## Configuration

Settings are saved in `BepInEx/config/com.pics0ul.valheim.theeyeofoden.cfg` upon first run or can be configured in-game using Configuration Manager.

### Key Settings

| Section | Setting | Default | Description |
| :--- | :--- | :--- | :--- |
| **General** | `Enabled` | `true` | Master switch for the radar. |
| **General** | `ToggleKey` | `None` | Optional hotkey to toggle radar on/off. |
| **General** | `RadarRange` | `120` | Detection radius in meters (`0` for unlimited loaded range). |
| **General** | `ShowOnSmallMap` | `true` | Show on HUD minimap. |
| **General** | `ShowOnLargeMap` | `true` | Show on fullscreen map. |
| **General** | `ClampToMinimapEdge` | `false` | Clamp off-screen entities to the minimap border like a compass. |
| **Filters** | `ShowHostiles` | `true` | Display hostile monsters. |
| **Filters** | `ShowAlerted` | `true` | Display alerted enemies. |
| **Filters** | `ShowPassives` | `true` | Display passive animals (Deer, Birds, etc.). |
| **Filters** | `ShowTamed` | `true` | Display tamed creatures. |
| **Filters** | `ShowBosses` | `true` | Display bosses. |
| **Filters** | `ShowOtherPlayers` | `true` | Display other players in multiplayer. |
| **Filters** | `ShowStarredOnly` | `false` | Filter to only 1-star, 2-star, and boss creatures. |
| **Visuals** | `DotSize` | `8` | Base diameter of radar dots in pixels. |
| **Visuals** | `BossScale` | `1.6` | Scale multiplier for boss dots. |
| **Visuals** | `StarredScale` | `1.3` | Scale multiplier for 1-star and 2-star creatures. |
| **Visuals** | `AlertedPulse` | `true` | Animate pulsing ring on alerted enemies. |
| **Visuals** | `ShowElevationIndicators`| `true` | Display subtle up/down chevrons for elevation differences. |
| **Visuals** | `DisplayMode` | `DotsOnly` | `DotsOnly`, `IconsWithDotsFallback`, or `IconsOnly`. |

---

## Building & Installing

### Prerequisites
- [.NET 8+ SDK](https://dotnet.microsoft.com/)
- Valheim & BepInEx installed

### Build Commands
Run from PowerShell in the project directory:

```powershell
# Build Release DLL
.\build.ps1

# Build and deploy directly to your r2modman profile
.\build.ps1 -Install

# Temporarily disable without uninstalling
.\build.ps1 -Disable

# Re-enable
.\build.ps1 -Enable
```
