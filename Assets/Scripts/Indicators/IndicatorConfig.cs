using UnityEngine;

namespace dutpekmezi
{
    [CreateAssetMenu(fileName = "IndicatorConfig", menuName = "Game/Scriptable Objects/Indicator/Indicator Config")]
    public class IndicatorConfig : ScriptableObject
    {
        public TargetIndicator targetIndicator;
    }
}