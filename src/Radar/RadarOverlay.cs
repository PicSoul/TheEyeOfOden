using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TheEyeofOden.Radar
{
    public static class RadarOverlay
    {
        private static GameObject _smallRootObj;
        private static GameObject _largeRootObj;
        private static RadarMarkerPool _smallPool;
        private static RadarMarkerPool _largePool;
        private static bool _initialized;

        public static void Initialize(Minimap minimap)
        {
            if (_initialized) return;

            RadarTextures.Initialize();

            if (minimap.m_pinRootSmall != null)
            {
                _smallRootObj = new GameObject("TheEyeOfOden_SmallRoot", typeof(RectTransform));
                _smallRootObj.transform.SetParent(minimap.m_pinRootSmall, false);
                RectTransform rt = _smallRootObj.GetComponent<RectTransform>();
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
                _smallRootObj.transform.SetAsFirstSibling(); // Render behind player arrow and standard pins

                _smallPool = new RadarMarkerPool(_smallRootObj.transform, 64);
            }

            if (minimap.m_pinRootLarge != null)
            {
                _largeRootObj = new GameObject("TheEyeOfOden_LargeRoot", typeof(RectTransform));
                _largeRootObj.transform.SetParent(minimap.m_pinRootLarge, false);
                RectTransform rt = _largeRootObj.GetComponent<RectTransform>();
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
                _largeRootObj.transform.SetAsFirstSibling();

                _largePool = new RadarMarkerPool(_largeRootObj.transform, 64);
            }

            _initialized = true;
        }

        public static void UpdateRadar(Minimap minimap)
        {
            if (!_initialized || minimap == null) return;

            if (!ModConfig.ModEnabled.Value || Player.m_localPlayer == null)
            {
                HideAll();
                return;
            }

            Minimap.MapMode mode = minimap.m_mode;
            RawImage mapImage;
            RadarMarkerPool activePool;

            if (mode == Minimap.MapMode.Small)
            {
                if (!ModConfig.ShowOnSmallMap.Value)
                {
                    HideAll();
                    return;
                }
                mapImage = minimap.m_mapImageSmall;
                activePool = _smallPool;
                _largePool?.HideAll();
            }
            else if (mode == Minimap.MapMode.Large)
            {
                if (!ModConfig.ShowOnLargeMap.Value)
                {
                    HideAll();
                    return;
                }
                mapImage = minimap.m_mapImageLarge;
                activePool = _largePool;
                _smallPool?.HideAll();
            }
            else
            {
                HideAll();
                return;
            }

            if (activePool == null || mapImage == null) return;

            activePool.BeginFrame();

            List<Character> allChars = Character.GetAllCharacters();
            if (allChars == null || allChars.Count == 0)
            {
                activePool.EndFrame(Time.time);
                return;
            }

            Rect uvRect = mapImage.uvRect;
            Rect transformRect = mapImage.rectTransform.rect;
            Player localPlayer = Player.m_localPlayer;
            bool clampEdge = ModConfig.ClampToMinimapEdge.Value && mode == Minimap.MapMode.Small;
            Vector2 mapCenter = new Vector2(transformRect.width * 0.5f, transformRect.height * 0.5f);
            float maxSmallRadius = Mathf.Min(transformRect.width, transformRect.height) * 0.48f;

            for (int i = 0; i < allChars.Count; i++)
            {
                Character character = allChars[i];
                if (character == null) continue;

                EntityClassification info = EntityClassifier.Classify(character, localPlayer);
                if (!info.Display) continue;

                Vector3 worldPos = character.transform.position;
                bool isVisible = IsPointVisible(minimap, worldPos, mapImage);

                if (!isVisible && !clampEdge)
                {
                    continue;
                }

                WorldToMapPoint(minimap, worldPos, out float mx, out float my);
                Vector2 localGuiPos = MapPointToLocalGuiPos(mx, my, uvRect, transformRect);

                if (!isVisible && clampEdge)
                {
                    Vector2 offsetFromCenter = localGuiPos - mapCenter;
                    if (offsetFromCenter.sqrMagnitude > maxSmallRadius * maxSmallRadius)
                    {
                        localGuiPos = mapCenter + offsetFromCenter.normalized * maxSmallRadius;
                    }
                }

                RadarMarker marker = activePool.GetMarker();
                EntityIconResolver.TryGetIcon(character, out Sprite icon);
                marker.Setup(info, icon);
                marker.SetPosition(localGuiPos);
            }

            activePool.EndFrame(Time.time);
        }

        public static void WorldToMapPoint(Minimap minimap, Vector3 p, out float mx, out float my)
        {
            int halfSize = minimap.m_textureSize / 2;
            mx = (p.x / minimap.m_pixelSize + halfSize) / minimap.m_textureSize;
            my = (p.z / minimap.m_pixelSize + halfSize) / minimap.m_textureSize;
        }

        public static bool IsPointVisible(Minimap minimap, Vector3 p, RawImage img)
        {
            WorldToMapPoint(minimap, p, out float mx, out float my);
            Rect uv = img.uvRect;
            return mx >= uv.xMin && mx <= uv.xMax && my >= uv.yMin && my <= uv.yMax;
        }

        public static Vector2 MapPointToLocalGuiPos(float mx, float my, Rect uvRect, Rect transformRect)
        {
            Vector2 result;
            result.x = ((mx - uvRect.xMin) / uvRect.width) * transformRect.width;
            result.y = ((my - uvRect.yMin) / uvRect.height) * transformRect.height;
            return result;
        }

        public static void HideAll()
        {
            _smallPool?.HideAll();
            _largePool?.HideAll();
        }

        public static void Destroy()
        {
            _smallPool?.Clear();
            _largePool?.Clear();
            _smallPool = null;
            _largePool = null;

            if (_smallRootObj != null)
            {
                Object.Destroy(_smallRootObj);
                _smallRootObj = null;
            }

            if (_largeRootObj != null)
            {
                Object.Destroy(_largeRootObj);
                _largeRootObj = null;
            }

            _initialized = false;
        }
    }
}
