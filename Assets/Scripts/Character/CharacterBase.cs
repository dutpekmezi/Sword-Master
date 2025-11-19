using UnityEngine;
using Utils.Signal;

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

        public void Initialize()
        {
            isDead = false;
            currentHealth = characterData.MaxHealth;
            currentEnergy = 0;

            SignalBus.Get<OnTakeDamage>().Subscribe(OnTakeDamageHandler);
        }

        public void Tick()
        {
            if (isDead) return;

            HandleInput();
            MoveCharacter();
        }

        private void HandleInput()
        {
            moveInput = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ).normalized;
        }

        private void MoveCharacter()
        {
            Vector2 targetVelocity = moveInput * characterData.MoveSpeed;

            moveVelocity = Vector2.Lerp(
                moveVelocity,
                targetVelocity,
                0.15f
            );

            rb.MovePosition(rb.position + moveVelocity * Utils.LogicTimer.LogicTimer.FixedDelta);
        }

        private void OnTakeDamageHandler(CharacterBase character, int dmg)
        {
            TakeDamage(dmg);
        }

        private void TakeDamage(int damageAmount)
        {
            if (isDead) return;

            SetHealth(-damageAmount);

            
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

            SignalBus.Get<OnStatsChange>().Invoke(this);
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

            SignalBus.Get<OnStatsChange>().Invoke(this);
        }

        public class OnTakeDamage : Signal<CharacterBase, int> {}
        public class OnStatsChange : Signal<CharacterBase> {}
    }
}
