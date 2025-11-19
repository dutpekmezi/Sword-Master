using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dutpekmezi.Services
{
    public enum SceneType
    {
        Base,
        Game,
        Menu,
        Other
    }

    [CreateAssetMenu(
        fileName = "SceneData",
        menuName = "Game/Scriptable Objects/Services/SceneData")]
    public class SceneData : ScriptableObject
    {
        public SceneType SceneType;

#if UNITY_EDITOR
        public SceneAsset SceneAsset;
#endif

        [SerializeField]
        private string _sceneName;

        public string SceneName =>
#if UNITY_EDITOR
            SceneAsset != null ? SceneAsset.name : _sceneName;
#else
            _sceneName;
#endif

        public Scene GetScene() => SceneManager.GetSceneByName(SceneName);

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (SceneAsset != null)
                _sceneName = SceneAsset.name;
        }
#endif
    }
}
