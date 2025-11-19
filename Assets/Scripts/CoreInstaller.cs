using Dutpekmezi.Services.SaveServices;
using Dutpekmezi.Services;
using dutpekmezi;
using UnityEngine;


using UnityEditor.SearchService;
using UnityEditor;

public class CoreInstaller : MonoBehaviour
{
    [SerializeField] private SceneServiceSettings _sceneServiceSettings;

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        InstallBindings();
    }

    private async void InstallBindings()
    {
        var sceneService = new SceneService(_sceneServiceSettings);

        var saveService = new SaveService(new EncryptedSaveHandler());

        _ = SceneService.Load(_sceneServiceSettings.GetSceneAssetByType(SceneType.Menu));
    }
}
