using System.Collections.Generic;
using UnityEngine;
using Utils.Signal;

namespace dutpekmezi
{
    public class CharacterBase : Entity
    {
        [Header("Assigned Data")]
        [SerializeField] private CharacterData characterData;

        [Header("References")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private BoxCollider2D col;

        [Header("Movement Settings")]
        [SerializeField] private float smoothMove = 10f;

        private Vector2 moveInput;
        private Vector2 moveVelocity;

        [SerializeField] private float currentEnergy;
        public float CurrentEnergy => currentEnergy;

        public bool isEnergyFull => currentEnergy >= GetStatValue(StatType.Energy); 

        public CharacterData CharacterData => characterData;

        public override void Initialize()
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

            //SignalBus.Get<OnTakeDamage>().Subscribe(OnTakeDamageHandler);

            SignalBus.Get<StatSystem.OnStatSelected>().Subscribe(ApplySelectedModifier);
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

        
    }
}
