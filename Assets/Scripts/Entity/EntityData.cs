using dutpekmezi;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace dutpekmezi
{
    public abstract class EntityData : ScriptableObject
    {
        public string Id;
        public string Name;
        public string Description;

        public Sprite Sprite;

        public List<BaseStatConfig> BaseStatConfigs;

        public Entity Prefab;

        public List<StatType> GetStatsType()
        {
            List<StatType> retunList = new List<StatType>();

            foreach (var stat in BaseStatConfigs)
            {
                retunList.Add(stat.BaseStat.Type);
            }

            return retunList;
        }

        public List<StatType> GetUpgradableStatsType()
        {
            List<StatType> retunList = new List<StatType>();

            foreach (var stat in BaseStatConfigs)
            {
                if (stat.IsUpgradable)
                    retunList.Add(stat.BaseStat.Type);
            }

            return retunList;
        }
    }
}