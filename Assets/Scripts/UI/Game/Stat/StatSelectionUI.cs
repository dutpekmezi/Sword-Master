using System.Collections.Generic;
using UnityEngine;
using Utils.Signal;

namespace dutpekmezi
{
    public class StatSelectionUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform parent;
        [SerializeField] private StatCardUI statCardPrefab;
        [SerializeField] private GameObject scnreenDim;

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

            scnreenDim.SetActive(false);

            var character = CharacterSystem.Instance.GetCurrentCharacter();

            List<StatType> availableStats = new List<StatType>(character.CharacterData.GetStatsType());

            for (int i = 0; i < StatSystem.Instance.StatConfigData.SelectableStatCount; i++)
            {
                var randomIndex = Random.Range(0, availableStats.Count);

                var randomStatType = availableStats[randomIndex];

                var randomModifier = StatSystem.Instance.CreateRandomModifier(randomStatType);

                var instance = Dutpekmezi.Services.PoolService.ObjectPoolManager.SpawnObject(statCardPrefab, parent);
                instance.Init(randomModifier);

                displayingStatCards.Add(instance);
                availableStats.Remove(randomStatType);
            }

        }

        private void HideStatsHandler(StatModifier statModifier)
        {
            HideStats();
        }

        private void HideStats()
        {
            scnreenDim.SetActive(false);

            if (displayingStatCards.Count > 0)
            {
                foreach (var statCard in displayingStatCards)
                {
                    Dutpekmezi.Services.PoolService.ObjectPoolManager.DeSpawn(statCard.gameObject);
                }

                displayingStatCards.Clear();
            }

            onSelecting = false;
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