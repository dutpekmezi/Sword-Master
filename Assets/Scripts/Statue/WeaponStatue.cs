using dutpekmezi;
using UnityEngine;
using Utils.Signal;

namespace dutpekmezi
{
    public class WeaponStatue : StatueBase
    {
        protected override void GetUpgrade()
        {
            SignalBus.Get<WeaponSystem.OnWeaponSelection>().Invoke();

            base.GetUpgrade();
        }
    }
}