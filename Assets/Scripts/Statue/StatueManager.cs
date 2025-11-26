using System.Collections.Generic;
using UnityEngine;
using Utils.Signal;

namespace dutpekmezi
{
    public class StatueManager : BaseSystem
    {
        private List<StatueBase> activeStatues = new List<StatueBase>();
        private List<StatStatue> activeStatStatues = new List<StatStatue>();
        private List<WeaponStatue> activeWeaponStatues = new List<WeaponStatue>();

        private StatueBase statStatue;
        private StatueBase weaponStatue;


        public List<StatueBase> ActiveStatues => activeStatues;
        public List<StatStatue> ActiveStatStatues => activeStatStatues;
        public List<WeaponStatue> ActiveWeaponStatues => activeWeaponStatues;
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
            var instance = Dutpekmezi.Services.PoolService.ObjectPoolManager.SpawnObject(statStatue, Vector2.zero);
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
            var instance = Dutpekmezi.Services.PoolService.ObjectPoolManager.SpawnObject(weaponStatue, Vector2.zero);
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

        public List<Transform> GetStatStatuesTransform()
        {
            var transformList = new List<Transform>();

            foreach (var instance in activeStatStatues)
            {
                transformList.Add(instance.transform);
            }

            return transformList;
        }

        public List<Transform> GetWeaponStatuesTransform()
        {
            var transformList = new List<Transform>();

            foreach (var instance in activeWeaponStatues)
            {
                transformList.Add(instance.transform);
            }

            return transformList;
        }

        public List<Transform> GetAllStatuesTransform()
        {
            var transformList = new List<Transform>();

            foreach (var instance in activeStatues)
            {
                transformList.Add(instance.transform);
            }

            return transformList;
        }

        public StatueBase DisposeStatue(StatueBase statue)
        {
            SignalBus.Get<OnStatueDispose>().Invoke(statue.transform);


            if (statue.Type == StatueType.Stat)
            {
                activeStatStatues.Remove((StatStatue)statue);
                activeStatues.Remove(statue);
                return statue;
            }

            if (statue.Type == StatueType.Weapon)
            {
                activeWeaponStatues.Remove((WeaponStatue)statue);
                activeStatues.Remove(statue);
                return statue;
            }

            return null;
        }

        public class OnStatueDispose : Signal<Transform> {}
    }
}