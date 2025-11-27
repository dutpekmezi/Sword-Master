using dutpekmezi;
using System.Collections.Generic;
using UnityEngine;

public class EntityData : ScriptableObject
{
    public string Id;
    public string Name;
    public string Description;

    public Sprite Sprite;

    public List<Stat> BaseStats;

    public Entity Prefab;

    public List<StatType> GetStatsType()
    {
        List<StatType> retunList = new List<StatType>();

        foreach (var stat in BaseStats)
        {
            retunList.Add(stat.Type);
        }

        return retunList;
    }
}
