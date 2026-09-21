using System.Collections.Generic;
using UnityEngine;

namespace TheEyeOfOden.Radar
{
    public class RadarMarkerPool
    {
        private readonly Transform _parent;
        private readonly List<RadarMarker> _pool = new List<RadarMarker>();
        private int _activeCount;

        public RadarMarkerPool(Transform parent, int initialCapacity = 64)
        {
            _parent = parent;
            for (int i = 0; i < initialCapacity; i++)
            {
                RadarMarker marker = RadarMarker.Create(_parent);
                marker.SetVisible(false);
                _pool.Add(marker);
            }
        }

        public void BeginFrame()
        {
            _activeCount = 0;
        }

        public RadarMarker GetMarker()
        {
            RadarMarker marker;
            if (_activeCount < _pool.Count)
            {
                marker = _pool[_activeCount];
            }
            else
            {
                marker = RadarMarker.Create(_parent);
                _pool.Add(marker);
            }

            _activeCount++;
            marker.SetVisible(true);
            return marker;
        }

        public void EndFrame(float time)
        {
            // Animate active markers
            for (int i = 0; i < _activeCount; i++)
            {
                _pool[i].UpdateAnimation(time);
            }

            // Hide inactive markers
            for (int i = _activeCount; i < _pool.Count; i++)
            {
                _pool[i].SetVisible(false);
            }
        }

        public void HideAll()
        {
            for (int i = 0; i < _pool.Count; i++)
            {
                _pool[i].SetVisible(false);
            }
            _activeCount = 0;
        }

        public void Clear()
        {
            for (int i = 0; i < _pool.Count; i++)
            {
                if (_pool[i] != null)
                {
                    Object.Destroy(_pool[i].gameObject);
                }
            }
            _pool.Clear();
            _activeCount = 0;
        }
    }
}
