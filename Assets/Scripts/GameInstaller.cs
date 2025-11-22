using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Utils.LogicTimer;
using Utils.Signal;

namespace dutpekmezi
{
    public class GameInstaller : MonoBehaviour
    {
        [Header("Data References")]
        [SerializeField] private CharacterDatas characterDatas;
        [SerializeField] private EnemyDatas enemyDatas;
        [SerializeField] private WeaponDatas weaponDatas;
        [SerializeField] private StatConfigData statConfigData;

        [Header("Wave Time Settings")]
        [SerializeField] private float preChaosDuration;
        [SerializeField] private float preChaosWaweSpawnRate;
        [SerializeField] private float preChaosGroupSpawnRate;

        [Header("Enemy Wave Settings")]
        [SerializeField] private int enemiesPerWave;
        [SerializeField] private float waveSpawnRadius;
        [SerializeField] private float waveSpawnDeflection;

        [Header("Enemy Group Wave Settings")]
        [SerializeField] private int enemiesPerGroup;
        [SerializeField] private float groupSpawnRadius;
        [SerializeField] private float groupSpawnDeflection;
        [SerializeField] private float enemyGroupRadius;
        [SerializeField] private float enemyGroupDeflection;

        [Header("UI settings")]
        [SerializeField] private WaveTimerUI waveTimerUI;

        private LogicTimer _logicTimer;
        private List<IDisposable> _disposables = new();


        private CharacterSystem _characterSystem;
        private EnemySystem _enemySystem;
        private WeaponSystem _weaponSystem;
        private StatSystem _statSystem;
        private WaveManager _waveManager;
        private UIManager _uiManager;

        private bool _initialized;

        private async void Awake()
        {
            await Initialize();
        }

        public async Task Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            InstallSystems();

            _logicTimer = Bind(new LogicTimer(OnFixedUpdate));
            _logicTimer.Start();

            await Task.CompletedTask;
        }

        private void InstallSystems()
        {
            _characterSystem = Bind(new CharacterSystem(characterDatas));
            _enemySystem = Bind(new EnemySystem(enemyDatas));
            _weaponSystem = Bind(new WeaponSystem(weaponDatas));
            _statSystem = Bind(new StatSystem(statConfigData));
            _uiManager = Bind(new UIManager(waveTimerUI));

            _waveManager = Bind(new WaveManager(
                _enemySystem,
                _characterSystem,
                enemiesPerWave,
                enemiesPerGroup,
                groupSpawnRadius,
                groupSpawnDeflection,
                enemyGroupRadius,
                enemyGroupDeflection,
                waveSpawnRadius,
                waveSpawnDeflection,
                preChaosDuration,
                preChaosWaweSpawnRate,
                preChaosGroupSpawnRate
            ));
        }

        private void OnFixedUpdate()
        {
            _characterSystem.Tick();
            _enemySystem.Tick();
            _weaponSystem.Tick();
            _statSystem.Tick();
            _waveManager.Tick();
            _uiManager.Tick();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
                SignalBus.Get<WeaponSystem.OnWeaponSelection>().Invoke();

            if (Input.GetKeyDown(KeyCode.J))
                SignalBus.Get<StatSystem.OnStatSelection>().Invoke();

            _logicTimer?.Update();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
                _logicTimer?.Pause();
            else
                _logicTimer?.Resume();
        }

        private T Bind<T>(T obj)
        {
            if (obj is IDisposable disposable)
                _disposables.Add(disposable);

            return obj;
        }

        private void OnDestroy()
        {
            foreach (var d in _disposables)
                d.Dispose();

            _logicTimer?.Dispose();
        }
    }
}
