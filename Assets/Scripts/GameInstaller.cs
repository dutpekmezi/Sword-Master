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
        [SerializeField] private WaveConfig waveConfig;
        [SerializeField] private IndicatorConfig indicatorConfig;

        [Header("Statue References")]
        [SerializeField] private StatueBase statStatue;
        [SerializeField] private StatueBase weaponStatue;

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
        private StatueManager _statueManager;
        private IndicatorManager _indicatorManager;

        private bool _initialized;

        public static GameInstaller Instance { get; private set; }

        private async void Awake()
        {
            Instance = this;

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
            _statueManager = Bind(new StatueManager(statStatue, weaponStatue));
            _indicatorManager = Bind(new IndicatorManager(indicatorConfig));

            _waveManager = Bind(new WaveManager(
                _enemySystem,
                _characterSystem,
                waveConfig

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
            _statueManager.Tick();
            _indicatorManager.Tick();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
                SignalBus.Get<WeaponSystem.OnWeaponSelection>().Invoke();

            if (Input.GetKeyDown(KeyCode.J))
                SignalBus.Get<StatSystem.OnStatSelection>().Invoke();

            if (Input.GetKeyDown(KeyCode.L))
                _characterSystem.GetCurrentCharacter().Gainlevel();

            _logicTimer?.Update();
        }

        public void OnApplicationPause(bool pause)
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
