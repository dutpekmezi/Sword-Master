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

        [Header("Movement Settings")]
        [SerializeField] private float smoothMove = 10f; // For smoother acceleration

        private Vector2 moveInput;
        private Vector2 moveVelocity;

        private bool isDead = false;

        [SerializeField] private int currentHealth;
        [SerializeField] private int currentEnergy;
        public int CurrentHealth => currentHealth;
        public int CurrentEnergy => currentEnergy;

        public bool isEnergyFull => currentEnergy >= characterData.MaxEnergy; 

        public Transform Transform => transform;

        public CharacterData CharacterData => characterData;

        public delegate void OnStatsChangeEvent(CharacterBase character);
        public event OnStatsChangeEvent OnStatsChange;

        public delegate void OnTakeDamageEvent(CharacterBase character);
        public event OnTakeDamageEvent OnTakeDamage;

        public delegate void OnKillEnemyEvent(CharacterBase character);
        public event OnKillEnemyEvent OnKillEnemy;

        private void Start()
        {
            Init();
        }

        private void Update()
        {
            if (isDead) return;

            HandleInput();
        }

        private void FixedUpdate()
        {
            if (isDead) return;

            MoveCharacter();
        }

        private void Init()
        {
            isDead = false;
            currentHealth = characterData.MaxHealth;
            OnStatsChange?.Invoke(this);
        }

        private void HandleInput()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            moveInput = new Vector2(moveX, moveY).normalized;
        }

        private void MoveCharacter()
        {
            // Target velocity based on input
            Vector2 targetVelocity = moveInput * characterData.MoveSpeed;

            // Smooth acceleration for natural movement
            moveVelocity = Vector2.Lerp(moveVelocity, targetVelocity, smoothMove * Time.fixedDeltaTime);

            // Apply to Rigidbody
            rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
        }

        public void TakeDamage(int damageAmount)
        {
            if (isDead) return;

            SetHealth(-damageAmount);

            OnTakeDamage?.Invoke(this);
        }

        private void SetHealth(int amount)
        {
            if (isDead) return;

            currentHealth += amount;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                isDead = true;
            }

            OnStatsChange?.Invoke(this);
        }

        private void SetEnergy(int amount)
        {
            if (isDead ||isEnergyFull) return;

            currentEnergy += amount;

            if (currentEnergy > characterData.MaxEnergy)
            {
                currentEnergy = characterData.MaxEnergy;
            }

            if (currentEnergy < 0)
            {
                currentEnergy = 0;
            }
        }
    }
}
