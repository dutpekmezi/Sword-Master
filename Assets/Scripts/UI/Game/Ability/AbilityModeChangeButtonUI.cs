using UnityEngine;
using Utils.Signal;

namespace dutpekmezi
{
    public class AbilityModeChangeButtonUI : MonoBehaviour
    {
        public void OnClick()
        {
            SignalBus.Get<AbilitySystem.OnAbilityModeChanging>().Invoke();
            SignalBus.Get<StatSystem.OnStatSelected>().Invoke(null);
        }
    }
}