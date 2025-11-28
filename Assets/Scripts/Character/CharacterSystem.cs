using System;
using Utils.Signal;
using Dutpekmezi.Services.PoolService;
using UnityEngine;

namespace dutpekmezi
{
    public class CharacterSystem : BaseSystem
    {
        private readonly CharacterDatas _characterDatas;

        private CharacterData _selectedCharacter;
        private CharacterBase _currentCharacter;

        public CharacterBase CurrentCharacter => _currentCharacter;

        public static CharacterSystem Instance { get; private set; }

        public CharacterSystem(CharacterDatas characterDatas)
        {
            Instance = this;

            _characterDatas = characterDatas;

            if (_characterDatas.Characters.Count == 1)
                _selectedCharacter = _characterDatas.Characters[0];

            OnInitialize();
        }

        protected override void OnInitialize()
        {
            CreateCharacter();
        }

        public override void Tick()
        {
            if (_currentCharacter != null)
            {
                _currentCharacter.Tick();
            }
        }

        public CharacterData GetCurrentCharacterData()
        {
            return _currentCharacter != null ? (CharacterData)_currentCharacter.EntityData : null;
        }

        public CharacterBase GetCurrentCharacter()
        {
            return _currentCharacter;
        }

        public CharacterBase CreateCharacter()
        {
            if (_selectedCharacter == null)
                return null;

            var prefab = _selectedCharacter.Prefab;

            var instance = Dutpekmezi.Services.PoolService.ObjectPoolManager.SpawnObject(prefab, Vector2.zero);
            instance.Initialize();

            _currentCharacter = (CharacterBase)instance;

            SignalBus.Get<OnCharacterSpawnedSignal>().Invoke(_currentCharacter);

            return _currentCharacter;
        }

        protected override void OnDispose()
        {
            if (_currentCharacter != null)
            {
                ObjectPoolManager.DeSpawn(_currentCharacter.gameObject);
                _currentCharacter = null;
            }
        }

        public class OnCharacterSpawnedSignal : Signal<CharacterBase> { }
    }
}
