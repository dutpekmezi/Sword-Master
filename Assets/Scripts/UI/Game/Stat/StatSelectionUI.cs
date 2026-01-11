using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Signal;


namespace dutpekmezi
{
    public class StatSelectionUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform parent;
        [SerializeField] private StatCardUI statCardPrefab;
        [SerializeField] private Image scnreenDim;
        [SerializeField] private GameObject abiltiyChangeButton;

        private bool onSelecting = false;

        private List<StatCardUI> displayingStatCards = new List<StatCardUI>();

        private void Start()
        {
            SignalBus.Get<StatSystem.OnStatSelection>().Subscribe(DisplayStats);
            SignalBus.Get<StatSystem.OnStatSelected>().Subscribe(HideStatsHandler);
        }

        private void DisplayStats()
        {
            if (onSelecting) return;

            onSelecting = true;

            scnreenDim.enabled = true;
            abiltiyChangeButton.SetActive(true);

            var character = CharacterSystem.Instance.GetCurrentCharacter();

            var modifiers = StatSystem.Instance.CreateSelectionModifiers(
                StatSystem.Instance.StatConfigData.SelectableStatCount,
                character.CurrentLevel
            );

            foreach (var randomModifier in modifiers)
            {
                var instance = Dutpekmezi.Services.PoolService.ObjectPoolManager.SpawnObject(statCardPrefab, parent);
                instance.Init(randomModifier);

                displayingStatCards.Add(instance);
            }

            GameInstaller.Instance.OnApplicationPause(true);

        }

        private void HideStatsHandler(StatModifier statModifier)
        {
            HideStats();
        }

        private void HideStats()
        {
            scnreenDim.enabled = false;
            abiltiyChangeButton.SetActive(false);

            if (displayingStatCards.Count > 0)
            {
                foreach (var statCard in displayingStatCards)
                {
                    Dutpekmezi.Services.PoolService.ObjectPoolManager.DeSpawn(statCard.gameObject);
                }

                displayingStatCards.Clear();
            }

            onSelecting = false;

            GameInstaller.Instance.OnApplicationPause(false);
        }

        private void OnWeaponSelectedHandler(WeaponData weaponData)
        {
            HideStats();
        }
        private void OnWeaponSelectionHendler()
        {
            DisplayStats();
        }
    }
}
