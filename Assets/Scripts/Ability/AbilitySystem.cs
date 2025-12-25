using UnityEngine;
using Utils.Signal;

namespace dutpekmezi
{
    public class AbilitySystem : BaseSystem
    {
        public enum AbilityMode { Character, Weapon }
        public AbilityMode CurrentMode { get; private set; }

        public static AbilitySystem Instance { get; private set; }

        public AbilitySystem(AbilityDatas abilityDatas)
        {
            Instance = this;

            OnInitialize();
        }

        protected override void OnInitialize()
        {
            CurrentMode = AbilityMode.Weapon;

            SignalBus.Get<OnAbilityModeChanging>().Unsubscribe(ToggleAbilityMode);
            SignalBus.Get<OnAbilityModeChanging>().Subscribe(ToggleAbilityMode);
        }

        private void ToggleAbilityMode()
        {
            if (CurrentMode == AbilityMode.Character)
                CurrentMode = AbilityMode.Weapon;
            else
                CurrentMode = AbilityMode.Character;

            Debug.Log("Current Ability Mode: " + CurrentMode);

            SignalBus.Get<OnAbilityModeChanged>().Invoke(CurrentMode);
        }

        protected override void OnDispose()
        {
            SignalBus.Get<OnAbilityModeChanging>().Unsubscribe(ToggleAbilityMode);
        }

        public class OnAbilityModeChanged : Signal<AbilityMode> { }
        public class OnAbilityModeChanging : Signal { }
    }
}