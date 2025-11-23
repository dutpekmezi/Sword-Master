using Dutpekmezi.Services.PoolService;
using NUnit.Framework;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using UnityEngine;

namespace dutpekmezi
{
    public class StatueManager : BaseSystem
    {
        private List<StatueBase> activeStatues;
        private List<StatueBase> activeStatStatues;
        private List<StatueBase> activeWeaponStatues;

        private StatueBase statStatue;
        private StatueBase weaponStatue;
        public static StatueManager Instance { get; private set; }
        public StatueManager(StatueBase statStatue, StatueBase weaponStatue)
        {
            Instance = this;

            this.statStatue = statStatue;
            this.weaponStatue = weaponStatue;

            OnInitialize();
        }

        protected override void OnInitialize()
        {
        }

        public override void Tick()
        {
        }

        protected override void OnDispose()
        {
            activeStatues.Clear();
            activeStatues.Clear();
        }

        public StatueBase CreateStatStatue()
        {
            var instance = Dutpekmezi.Services.PoolService.ObjectPoolManager.SpawnObject(statStatue, WaveManager.Instance.WaveEntitiesHolder);
            instance.Init(StatueType.Stat);

            activeStatues.Add(instance);
            activeStatStatues.Add(instance);

            return instance;
        }

        public StatueBase CreateWeaponStatue()
        {
            var instance = Dutpekmezi.Services.PoolService.ObjectPoolManager.SpawnObject(weaponStatue, WaveManager.Instance.WaveEntitiesHolder);
            instance.Init(StatueType.Weapon);

            activeStatues.Add(instance);
            activeWeaponStatues.Add(instance);

            return instance;
        }
    }
}