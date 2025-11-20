using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Signal;
using static dutpekmezi.WeaponSystem;

namespace dutpekmezi
{
    public class WeaponSelectionUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform parent;
        [SerializeField] private WeaponCardUI weaponCardPrefab;
        [SerializeField] private GameObject scnreenDim;

        private List<WeaponCardUI> displayingWeaponCards = new List<WeaponCardUI>();

        private void Start()
        {
            SignalBus.Get<OnWeaponSelected>().Subscribe(OnWeaponSelectedHandler);

            SignalBus.Get<OnWeaponSelection>().Subscribe(OnWeaponSelectionHendler);
        }
        public void DisplayWeapons()
        {
            scnreenDim.SetActive(true);

            var selectedWeapons = new List<WeaponData>();

            selectedWeapons = WeaponSystem.Instance.GetRandomWeaponsData(2);

            foreach (var weapon in selectedWeapons)
            {
                var instance = Dutpekmezi.Services.PoolService.ObjectPoolManager.SpawnObject(weaponCardPrefab, parent);
                instance.Init(weapon);

                displayingWeaponCards.Add(instance);
            }
        }

        public void HideWeapons()
        {
            scnreenDim.SetActive(false);

            if (displayingWeaponCards.Count > 0)
            {
                foreach (var weaponCard in displayingWeaponCards)
                {
                    Dutpekmezi.Services.PoolService.ObjectPoolManager.DeSpawn(weaponCard.gameObject);
                }

                displayingWeaponCards.Clear();
            } 
        }

        private void OnWeaponSelectedHandler(WeaponData weaponData)
        {
            HideWeapons();
        }
        private void OnWeaponSelectionHendler()
        {
            DisplayWeapons();
        }


    }
}