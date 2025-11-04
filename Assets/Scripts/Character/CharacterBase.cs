using UnityEngine;

namespace dutpekmezi
{
    public class CharacterBase : MonoBehaviour
    {
        [Header("Assigned Data")]
        [SerializeField] private CharacterData characterData;

        [Header("References")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private BoxCollider2D col;

        [SerializeField] private int currentHealth; 
        public int CurrentHealth => currentHealth;

        public Transform Transform => transform;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            currentHealth = characterData.MaxHealth;
        }

        public void TakeDamage(int damageAmount)
        {
            SetHealth(-damageAmount);
        }

        private void SetHealth(int amount)
        {
            currentHealth += amount;
        }
    }
}