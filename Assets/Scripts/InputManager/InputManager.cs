using UnityEngine;
using Utils.Signal;

namespace dutpekmezi
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(Instance);
            }

            Instance = this;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnAbiltiyButtonClick();
            }
        }

        public void OnAbiltiyButtonClick()
        {
            Debug.Log("Clicked");
            SignalBus.Get<OnAbilityButtonClick>().Invoke();
        }

        public class OnAbilityButtonClick : Signal { }
    }
}