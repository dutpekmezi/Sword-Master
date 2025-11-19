using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Utils.LogicTimer;

namespace dutpekmezi
{
    public class GameInstaller : MonoBehaviour
    {
        [Header("Data References")]
        [SerializeField] private CharacterDatas characterDatas;
        [SerializeField] private EnemyDatas enemyDatas;
        [SerializeField] private WeaponDatas weaponDatas;

        [Header("Wave Settings")]
        [SerializeField] private int enemiesPerWave;
        [SerializeField] private float waveSpawnRadius;
        [SerializeField] private float waveSpawnDeflection;

        [Header("Enemy Group Wave Settings")]
        [SerializeField] private int enemiesPerGroup;
        [SerializeField] private float groupSpawnRadius;
        [SerializeField] private float groupSpawnDeflection;
        [SerializeField] private float enemyGroupRadius;
        [SerializeField] private float enemyGroupDeflection;

        private LogicTimer _logicTimer;
        private List<IDisposable> _disposables = new();


        private CharacterSystem _characterSystem;
        private EnemySystem _enemySystem;
        private WeaponSystem _weaponSystem;
        private WaveManager _waveManager;

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
                waveSpawnDeflection
            ));
        }

        private void OnFixedUpdate()
        {
            _characterSystem.Tick();
            _enemySystem.Tick();
            _weaponSystem.Tick();
            _waveManager.Tick();
        }

        private void FixedUpdate()
        {
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
