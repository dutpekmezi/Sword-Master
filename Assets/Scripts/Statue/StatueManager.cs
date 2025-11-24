using Dutpekmezi.Services.PoolService;
using NUnit.Framework;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using UnityEngine;

namespace dutpekmezi
{
    public class StatueManager : BaseSystem
    {
        private List<StatueBase> activeStatues = new List<StatueBase>();
        private List<StatStatue> activeStatStatues = new List<StatStatue>();
        private List<WeaponStatue> activeWeaponStatues = new List<WeaponStatue>();

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

        //-------------------STAT STATUE----------------------------//

        public StatueBase CreateStatStatue()
        {
            var instance = Dutpekmezi.Services.PoolService.ObjectPoolManager.SpawnObject(statStatue, WaveManager.Instance.WaveEntitiesHolder);
            instance.Init(StatueType.Stat);

            activeStatues.Add(instance);
            activeStatStatues.Add((StatStatue)instance);

            return instance;
        }

        public List<StatueBase> CreateStatStatues(int count)
        {
            var createdStatues = new List<StatueBase>();

            for (int i = 0; i < count; i++)
            {
                var instance = CreateStatStatue();
                createdStatues.Add(instance);
            }

            return createdStatues;
        }

        //-------------------WEAPON STATUE----------------------------//

        public StatueBase CreateWeaponStatue()
        {
            var instance = Dutpekmezi.Services.PoolService.ObjectPoolManager.SpawnObject(weaponStatue, WaveManager.Instance.WaveEntitiesHolder);
            instance.Init(StatueType.Weapon);

            activeStatues.Add(instance);
            activeWeaponStatues.Add((WeaponStatue)instance);

            return instance;
        }

        public List<StatueBase> CreateWeaponStatues(int count)
        {
            var createdStatues = new List<StatueBase>();

            for (int i = 0; i < count; i++)
            {
                var instance = CreateWeaponStatue();
                createdStatues.Add(instance);
            }

            return createdStatues;
        }
    }
}