using System.Collections.Generic;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class TimeManager : Singleton<TimeManager>
    {
        private const int LAYER_DEFAULT = 0;
        private const float PHYSICS_TIMESTEP = 0.02f;

        private class TimeData
        {
            private readonly float to;
            private readonly float from;

            private readonly float duration;
            private readonly float startTime;

            public TimeData(float duration, float to, float from = 1.0f)
            {
                this.to = to;
                this.from = from;

                this.duration = duration;
                startTime = Time.time;
            }

            public float Get()
            {
                if (Mathf.Approximately(duration, 0f)) return to;

                float t = (Time.time - startTime) / duration;
                return Mathf.SmoothStep(from, to, t);
            }
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private Dictionary<int, TimeData> timeScales = new Dictionary<int, TimeData>();
        private float iterateTime;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void SetTimeScale(float timeScale, int layer = LAYER_DEFAULT)
        {
            timeScales[layer] = new TimeData(0f, timeScale);
            RecalculateTimeScale();
        }

        public void SetSmoothTimeScale(float timeScale, float duration, int layer = LAYER_DEFAULT)
        {
            iterateTime = Mathf.Max(iterateTime, Time.time + duration);

            float from = 1.0f;
            if (timeScales.ContainsKey(layer))
            {
                from = timeScales[layer].Get();
            }

            timeScales[layer] = new TimeData(duration, timeScale, from);
        }

        // UPDATE METHOD: -------------------------------------------------------------------------

        private void Update()
        {
            if (Time.time > iterateTime) return;
            RecalculateTimeScale();
        }

        private void RecalculateTimeScale()
        {
            float scale = timeScales.Count > 0 ? 99f : 1.0f;
            foreach (KeyValuePair<int, TimeData> item in timeScales)
            {
                scale = Mathf.Min(scale, item.Value.Get());
            }

            Time.timeScale = scale;
            Time.fixedDeltaTime = PHYSICS_TIMESTEP * scale;
        }
    }
}