using UnityEngine;

namespace dutpekmezi
{
    public class CharacterSystem : MonoBehaviour
    {
        [SerializeField] private CharacterDatas characterDatas;

        private CharacterData selectedCharacter;

        private CharacterBase currentCharacter;

        public CharacterBase CurrentCharacter => currentCharacter;

        private static CharacterSystem instance;
        public static CharacterSystem Instance => instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(instance);
            }

            instance = this;

            if (characterDatas.Characters.Count == 1)
            {
                selectedCharacter = characterDatas.Characters[0];
            }
        }

        private void Start()
        {
            CreateCharacter();
        }

        public CharacterBase GetCharacter()
        {
            if (selectedCharacter != null || characterDatas != null || currentCharacter != null)
            {
                return currentCharacter;
            }

            return null;
        }

        public CharacterBase CreateCharacter()
        {
            if (selectedCharacter != null || characterDatas != null)
            {
                var characterPrefab = selectedCharacter.Prefab;

                CharacterBase instance = Instantiate(characterPrefab);

                currentCharacter = instance;

                return instance;
            }

            return null;
        }

        public Transform GetCharacterTransform()
        {
            if (selectedCharacter != null || characterDatas != null || currentCharacter != null)
            {
                return currentCharacter.Transform;
            }

            return null;
        }
    }
}