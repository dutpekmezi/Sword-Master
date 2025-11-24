using UnityEngine;
using Utils.LogicTimer;
using Utils.Signal;

namespace dutpekmezi
{
    public class StatStatue : StatueBase
    {
        protected override void GetUpgrade()
        {
            SignalBus.Get<StatSystem.OnStatSelection>().Invoke();

            base.GetUpgrade();
        }
    }
}