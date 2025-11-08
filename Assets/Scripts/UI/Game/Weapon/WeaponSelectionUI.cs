using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace dutpekmezi
{
    public class WeaponSelectionUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform parent;
        [SerializeField] private WeaponCardUI weaponCardPrefab;
        [SerializeField] private GameObject scnreenDim;

        private List<WeaponCardUI> displayingWeaponCards = new List<WeaponCardUI>();
        public void DisplayWeapons()
        {
            scnreenDim.SetActive(true);

            var selectedWeapons = new List<WeaponData>();

            selectedWeapons = WeaponSystem.Instance.GetRandomWeaponsData(2);

            foreach (var weapon in selectedWeapons)
            {
                var instance = Instantiate(weaponCardPrefab, parent);
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
                    Destroy(weaponCard.gameObject);
                }

                displayingWeaponCards.Clear();
            } 
        }
    }
}