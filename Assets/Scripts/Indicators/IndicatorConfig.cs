using UnityEngine;
using UnityEngine.Serialization;

namespace dutpekmezi
{
    [CreateAssetMenu(fileName = "IndicatorConfig", menuName = "Game/Scriptable Objects/Indicator/Indicator Config")]
    public class IndicatorConfig : ScriptableObject
    {
        [FormerlySerializedAs("targetIndicator")]
        public TargetIndicator statueIndicator;
        public TargetIndicator chestIndicator;
    }
}
