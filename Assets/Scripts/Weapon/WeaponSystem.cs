using System;
using Utils.Signal;
using Utils.Logger;
using Dutpekmezi.Services.PoolService;
using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

namespace dutpekmezi
{
    public class WeaponSystem : BaseSystem
    {
        private WeaponDatas weaponDatas;

        private WeaponBase _currentWeapon;
        private Transform _characterTransform;

        public static WeaponSystem Instance { get; private set; }

        public WeaponSystem(WeaponDatas datas)
        {
            Instance = this;

            weaponDatas = datas;

            OnInitialize();
        }

        protected override void OnInitialize()
        {
            SignalBus.Get<CharacterSystem.OnCharacterSpawnedSignal>()
                     .Subscribe(OnCharacterSpawned);
        }

        private void OnCharacterSpawned(CharacterBase character)
        {
            _characterTransform = character.Transform;
        }

        public override void Tick()
        {
            if (_currentWeapon != null)
                _currentWeapon.Tick();
        }

        public WeaponData EquipWeapon(WeaponData weaponData)
        {
            if (_characterTransform == null)
                return null;

            if (_currentWeapon != null)
                ObjectPoolManager.DeSpawn(_currentWeapon.gameObject);

            var go = ObjectPoolManager.SpawnObject(weaponData.Prefab, _characterTransform);
            go.transform.localPosition = Vector3.zero;

            _currentWeapon = go.GetComponent<WeaponBase>();

            SignalBus.Get<OnWeaponEquippedSignal>().Invoke(weaponData);

            return weaponData;
        }

        public List<WeaponData> GetRandomWeaponsData(int amount = 1)
        {
            var clone = new List<WeaponData>(weaponDatas.weapons);

            List<WeaponData> result = new List<WeaponData>();

            for (int i = 0; i < amount; i++)
            {
                if (clone.Count == 0) break;

                int idx = UnityEngine.Random.Range(0, clone.Count);
                result.Add(clone[idx]);
                clone.RemoveAt(idx);
            }

            return result;
        }

        protected override void OnDispose()
        {
            if (_currentWeapon != null)
                ObjectPoolManager.DeSpawn(_currentWeapon.gameObject);

            _currentWeapon = null;
        }

        public class OnWeaponEquippedSignal : Signal<WeaponData> { }
    }
}
