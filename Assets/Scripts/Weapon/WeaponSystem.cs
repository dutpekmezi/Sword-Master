using System.Collections.Generic;
using UnityEngine;

namespace dutpekmezi
{
    public class WeaponSystem : MonoBehaviour
    {
        [SerializeField] private WeaponDatas weaponDatas;

        [SerializeField] private WeaponSelectionUI weaponSelectionUI;

        private WeaponBase currentWeapon;
        private WeaponData selectedWeapon;

        private static WeaponSystem instance;
        public static WeaponSystem Instance => instance;

        public WeaponSelectionUI WeaponSelectionUI => weaponSelectionUI;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(instance);
            }

            instance = this;
        }

        private void Start()
        {
           if (selectedWeapon != null) EquipWeapon(selectedWeapon);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                if (weaponSelectionUI == null) return;

                weaponSelectionUI.DisplayWeapons();
            }
        }

        public List<WeaponData> GetAllWeaponsData()
        {
            return weaponDatas.weapons;
        }

        public List<WeaponData> GetRandomWeaponsData(int count = 1)
        {
            List<WeaponData> weaponsClone = new List<WeaponData>();
            List<WeaponData> randomWeapons = new List<WeaponData>();

            weaponsClone = weaponDatas.weapons;

            for (int i = 0; i < count; i++)
            {
                if (weaponsClone.Count <= 0) break;

                int randomIndex = Random.Range(0, count);

                WeaponData randomWeapon = weaponsClone[randomIndex];

                randomWeapons.Add(randomWeapon);

                if (weaponsClone.Count > 0) weaponsClone.Remove(randomWeapon);
            }

            return randomWeapons;
        }

        public WeaponData EquipWeapon(WeaponData weaponDataToEquip)
        {
            selectedWeapon = weaponDataToEquip;

            if (currentWeapon != null)
            {
                Destroy(currentWeapon);
            }

            WeaponBase instance = Instantiate(selectedWeapon.Prefab, CharacterSystem.Instance.GetCurrentCharacterTransform());

            currentWeapon = instance;

            return weaponDataToEquip;
        }
    }
}