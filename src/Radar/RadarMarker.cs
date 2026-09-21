using UnityEngine;
using UnityEngine.UI;

namespace TheEyeOfOden.Radar
{
    public class RadarMarker : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private Image _dotImage;

        private GameObject _ringObj;
        private RectTransform _ringRectTransform;
        private Image _ringImage;

        private GameObject _elevationObj;
        private RectTransform _elevationRectTransform;
        private Image _elevationImage;

        private bool _isAlerted;
        private bool _isBoss;
        private float _baseSize;
        private Color _baseColor;

        public static RadarMarker Create(Transform parent)
        {
            GameObject root = new GameObject("RadarMarker", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            root.transform.SetParent(parent, false);

            RadarMarker marker = root.AddComponent<RadarMarker>();
            marker.Initialize(root);
            return marker;
        }

        private void Initialize(GameObject root)
        {
            _rectTransform = root.GetComponent<RectTransform>();
            _rectTransform.anchorMin = Vector2.zero;
            _rectTransform.anchorMax = Vector2.zero;
            _rectTransform.pivot = new Vector2(0.5f, 0.5f);
            _rectTransform.localScale = Vector3.one;

            _dotImage = root.GetComponent<Image>();
            _dotImage.raycastTarget = false;
            _dotImage.sprite = RadarTextures.CircleSprite;

            // 1. Ring Child (for Bosses and Alerted enemies)
            _ringObj = new GameObject("Ring", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            _ringObj.transform.SetParent(root.transform, false);
            _ringRectTransform = _ringObj.GetComponent<RectTransform>();
            _ringRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            _ringRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            _ringRectTransform.pivot = new Vector2(0.5f, 0.5f);
            _ringRectTransform.localScale = Vector3.one;

            _ringImage = _ringObj.GetComponent<Image>();
            _ringImage.raycastTarget = false;
            _ringImage.sprite = RadarTextures.RingSprite;
            _ringObj.SetActive(false);

            // 2. Elevation Chevron Child
            _elevationObj = new GameObject("Elevation", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            _elevationObj.transform.SetParent(root.transform, false);
            _elevationRectTransform = _elevationObj.GetComponent<RectTransform>();
            _elevationRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            _elevationRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            _elevationRectTransform.pivot = new Vector2(0.5f, 0.5f);
            _elevationRectTransform.localScale = Vector3.one;

            _elevationImage = _elevationObj.GetComponent<Image>();
            _elevationImage.raycastTarget = false;
            _elevationObj.SetActive(false);
        }

        public void Setup(in EntityClassification info, Sprite icon)
        {
            _isAlerted = info.IsAlerted;
            _isBoss = info.IsBoss;
            _baseColor = info.Color;

            float dotSize = ModConfig.DotSize.Value * info.Scale;
            _baseSize = dotSize;

            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, dotSize);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dotSize);

            // Icon vs Dot
            if (icon != null)
            {
                _dotImage.sprite = icon;
                _dotImage.color = Color.white; // Keep original icon colors
            }
            else
            {
                _dotImage.sprite = RadarTextures.CircleSprite;
                _dotImage.color = _baseColor;
            }

            // Ring (Alerted enemies or Bosses)
            bool showRing = (_isAlerted && ModConfig.AlertedPulse.Value) || _isBoss;
            _ringObj.SetActive(showRing);
            if (showRing)
            {
                float ringSize = dotSize * 1.6f;
                _ringRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ringSize);
                _ringRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ringSize);
                _ringImage.color = _isBoss ? ModConfig.BossColor.Value : ModConfig.AlertedColor.Value;
            }

            // Elevation Indicator
            bool showElevation = ModConfig.ShowElevationIndicators.Value &&
                                 Mathf.Abs(info.ElevationDiff) >= ModConfig.ElevationThreshold.Value;
            _elevationObj.SetActive(showElevation);
            if (showElevation)
            {
                bool above = info.ElevationDiff > 0f;
                _elevationImage.sprite = above ? RadarTextures.ChevronUpSprite : RadarTextures.ChevronDownSprite;
                _elevationImage.color = _baseColor;

                float chevSize = Mathf.Max(6f, dotSize * 0.8f);
                _elevationRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, chevSize);
                _elevationRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, chevSize);

                float yOffset = above ? (dotSize * 0.7f + chevSize * 0.5f) : (-dotSize * 0.7f - chevSize * 0.5f);
                _elevationRectTransform.anchoredPosition = new Vector2(0f, yOffset);
            }
        }

        public void SetPosition(Vector2 localGuiPos)
        {
            _rectTransform.anchoredPosition = localGuiPos;
        }

        public void UpdateAnimation(float time)
        {
            if (!_isAlerted || !ModConfig.AlertedPulse.Value) return;

            // Subtle pulsing effect for alerted enemies
            float pulse = 1f + 0.25f * Mathf.Sin(time * 7f);
            float currentSize = _baseSize * pulse;
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentSize);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentSize);

            if (_ringObj.activeSelf)
            {
                float ringScale = 1.3f + 0.35f * Mathf.Sin(time * 7f);
                _ringRectTransform.localScale = new Vector3(ringScale, ringScale, 1f);
            }
        }

        public void SetVisible(bool visible)
        {
            if (gameObject.activeSelf != visible)
            {
                gameObject.SetActive(visible);
            }
        }
    }
}
