using UnityEngine;

namespace TheEyeOfOden.Radar
{
    public static class RadarTextures
    {
        public static Sprite CircleSprite { get; private set; }
        public static Sprite RingSprite { get; private set; }
        public static Sprite ChevronUpSprite { get; private set; }
        public static Sprite ChevronDownSprite { get; private set; }

        private static bool _initialized;

        public static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            CircleSprite = CreateCircleSprite(64, 30f, 1.5f);
            RingSprite = CreateRingSprite(64, 30f, 23f, 1.5f);
            ChevronUpSprite = CreateChevronSprite(32, pointingUp: true);
            ChevronDownSprite = CreateChevronSprite(32, pointingUp: false);
        }

        private static Sprite CreateCircleSprite(int size, float radius, float feather)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = "TheEyeOfOden_Circle",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            Color[] pixels = new Color[size * size];
            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center);
                    float alpha = Mathf.Clamp01((radius - dist) / feather);
                    pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply(false, true); // make unreadable to save VRAM

            return Sprite.Create(
                tex,
                new Rect(0f, 0f, size, size),
                new Vector2(0.5f, 0.5f),
                100f);
        }

        private static Sprite CreateRingSprite(int size, float outerRadius, float innerRadius, float feather)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = "TheEyeOfOden_Ring",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            Color[] pixels = new Color[size * size];
            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center);
                    float outerAlpha = Mathf.Clamp01((outerRadius - dist) / feather);
                    float innerAlpha = Mathf.Clamp01((dist - innerRadius) / feather);
                    float alpha = outerAlpha * innerAlpha;
                    pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply(false, true);

            return Sprite.Create(
                tex,
                new Rect(0f, 0f, size, size),
                new Vector2(0.5f, 0.5f),
                100f);
        }

        private static Sprite CreateChevronSprite(int size, bool pointingUp)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = pointingUp ? "TheEyeOfOden_ChevronUp" : "TheEyeOfOden_ChevronDown",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            Color[] pixels = new Color[size * size];
            float midX = (size - 1) * 0.5f;

            for (int y = 0; y < size; y++)
            {
                float actualY = pointingUp ? y : (size - 1 - y);

                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Abs(x - midX);
                    // Peak at actualY = 24, sloping down to actualY = 8 at edges
                    float lineY = 24f - dx * 1.3f;
                    float dist = Mathf.Abs(actualY - lineY);
                    float alpha = Mathf.Clamp01((3.0f - dist) / 1.2f);

                    // Cut off at lower bounds
                    if (actualY < lineY - 4f) alpha = 0f;

                    pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply(false, true);

            return Sprite.Create(
                tex,
                new Rect(0f, 0f, size, size),
                new Vector2(0.5f, 0.5f),
                100f);
        }
    }
}
