using UnityEngine;
using Utils.Signal;

namespace dutpekmezi
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnAbiltiyButtonClick();
            }
        }

        private void OnAbiltiyButtonClick()
        {
            SignalBus.Get<OnAbilityButtonClick>().Invoke();
        }

        public class OnAbilityButtonClick : Signal { }
    }
}