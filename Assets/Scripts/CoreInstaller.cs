using Dutpekmezi.Services;
using Dutpekmezi.Services.SaveServices;
using UnityEngine;

namespace dutpekmezi
{
    public class CoreInstaller : MonoBehaviour
    {
        [SerializeField] private SceneServiceSettings _sceneServiceSettings;

        private void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            InstallBindings();
        }

        private void InstallBindings()
        {
            var sceneService = new SceneService(_sceneServiceSettings);

            var saveService = new SaveService(new EncryptedSaveHandler());
        }
    }
}
