using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace dutpekmezi
{
    public class WaveTimerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timeText;

        public void UpdateWaveTimer()
        {
            if (WaveManager.Instance == null) return;

            float totalSeconds = WaveManager.Instance.CurrentTimer;

            if (totalSeconds < 0) totalSeconds = 0;

            int minutes = Mathf.FloorToInt(totalSeconds / 60f);
            int seconds = Mathf.FloorToInt(totalSeconds % 60f);

            string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);

            timeText.text = timeString;

            if (WaveManager.Instance.CurrentWaveState == WaveState.Chaos)
            {
                timeText.color = Color.red;
            }
        }
    }
}