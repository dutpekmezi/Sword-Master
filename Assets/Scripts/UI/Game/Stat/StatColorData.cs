using UnityEngine;
using System.Collections.Generic;
using dutpekmezi;

namespace dutpekmezi
{
    [System.Serializable]
    public struct StatTypeColor
    {
        public StatType Type;
        public Color Color;
    }

    [CreateAssetMenu(fileName = "StatColorData", menuName = "Game/Scriptable Objects/Stat/Stat Color Data")]
    public class StatColorData : ScriptableObject
    {
        public List<StatTypeColor> StatColors;

        private Dictionary<StatType, Color> _colorLookup;

        public void InitializeLookup()
        {
            if (_colorLookup == null)
            {
                _colorLookup = new Dictionary<StatType, Color>();
                foreach (var item in StatColors)
                {
                    if (!_colorLookup.ContainsKey(item.Type))
                    {
                        _colorLookup.Add(item.Type, item.Color);
                    }
                    else
                    {
                        Debug.LogWarning($"Duplicate StatType found in StatColorData: {item.Type}");
                    }
                }
            }
        }

        public Color GetColor(StatType type)
        {
            if (_colorLookup == null) InitializeLookup();

            if (_colorLookup.TryGetValue(type, out Color color))
            {
                return color;
            }

            return Color.white;
        }
    }
}