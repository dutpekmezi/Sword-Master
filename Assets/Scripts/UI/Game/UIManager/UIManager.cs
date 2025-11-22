using UnityEngine;

namespace dutpekmezi
{
    public class UIManager : BaseSystem
    {
        private WaveTimerUI waveTimerUI;

        public UIManager(WaveTimerUI waveTimerUI)
        {
            this.waveTimerUI = waveTimerUI;

            OnInitialize();
        }

        protected override void OnInitialize()
        {

        }

        public override void Tick()
        {
            waveTimerUI.UpdateWaveTimer();
        }
    }
}