using System.Collections.Generic;
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

        private Dictionary<StatType, Stat> _runtimeStats = new Dictionary<StatType, Stat>();

        private bool isDead = false;

        [SerializeField] private float currentHealth;
        [SerializeField] private float currentEnergy;
        public float CurrentHealth => currentHealth;
        public float CurrentEnergy => currentEnergy;

        public bool isEnergyFull => currentEnergy >= GetStatValue(StatType.Energy); 

        public Transform Transform => transform;

        public CharacterData CharacterData => characterData;

        public void Initialize()
        {
            isDead = false;

            _runtimeStats.Clear();

            foreach (var baseStat in characterData.BaseStats)
            {
                Stat runtimeStat = new Stat(baseStat.BaseValue);

                _runtimeStats.Add(baseStat.Type, runtimeStat);
            }

            currentHealth = (int)GetStatValue(StatType.MaxHealth);
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
            Vector2 targetVelocity = moveInput * GetStatValue(StatType.MoveSpeed);

            moveVelocity = Vector2.Lerp(
                moveVelocity,
                targetVelocity,
                0.15f
            );

            rb.MovePosition(rb.position + moveVelocity * Utils.LogicTimer.LogicTimer.FixedDelta);
        }

        public float GetStatValue(StatType type)
        {
            if (_runtimeStats.TryGetValue(type, out Stat stat))
            {
                return stat.Value;
            }
            return 0f;
        }

        private void OnTakeDamageHandler(CharacterBase character, int dmg)
        {
            if (isDead) return;

            TakeDamage(dmg);
        }

        private void TakeDamage(int damageAmount)
        {
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
            if (isDead || isEnergyFull) return;

            currentEnergy += amount;

            if (currentEnergy > GetStatValue(StatType.Energy))
            {
                currentEnergy = GetStatValue(StatType.Energy);
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
