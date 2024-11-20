using UnityEngine;

namespace Csce552
{
    [System.Serializable]
    public struct JumpStats
    {
        public float InitialVelocity;
        public float HoldForce;
        public float Time;

        public static JumpStats FromMinMaxHeight(float yMin, float yMax, float g)
        {
            float initialVelocity = Mathf.Sqrt(2f * g * yMin);
            float holdForce = g * (1f - yMin / yMax);
            float time = initialVelocity / (g - holdForce);
            return new JumpStats()
            {
                InitialVelocity = initialVelocity,
                HoldForce = holdForce,
                Time = time,
            };
        }

        public static JumpStats FromInitialVelAndMultiplier(float initialVelocity, float g, float heightMultiplier)
        {
            float yMin = (initialVelocity * initialVelocity) / (2 * g);
            float yMax = yMin * heightMultiplier;
            float holdForce = g * (1f - yMin / yMax);
            float time = initialVelocity / (g - holdForce);
            return new JumpStats()
            {
                InitialVelocity = initialVelocity,
                HoldForce = holdForce,
                Time = time,
            };
        }
    }
}