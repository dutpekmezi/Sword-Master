using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dutpekmezi.Services
{
    public class SceneService
    {
        private static SceneServiceSettings _settings;
        private static readonly HashSet<string> _loadedSceneNames = new();
        private static bool _isInitialized;

        public static event Action<string> OnSceneLoaded;
        public static event Action<string> OnSceneUnloaded;


        public SceneService(SceneServiceSettings settings)
        {
            Initialize(settings);
        }

        private static void Initialize(SceneServiceSettings settings)
        {
            if (_isInitialized)
                return;

            if (settings == null)
            {
                Debug.LogError("[SceneService] Settings asset is missing!");
                return;
            }

            _settings = settings;
            _isInitialized = true;

            if (_settings.BaseSceneData.SceneAsset == null)
            {
                Debug.LogError("[SceneService] Base scene reference is empty.");
                return;
            }

            string baseSceneName = _settings.BaseSceneData.SceneName;

            // Always load the base scene as the main scene
            SceneManager.LoadScene(baseSceneName, LoadSceneMode.Single);
            _loadedSceneNames.Clear();
            _loadedSceneNames.Add(baseSceneName);

#if UNITY_EDITOR
            // If test mode is enabled, go directly to the test scene.
            if (_settings.TestMode && _settings.TestSceneData.SceneAsset != null)
            {
                Load(_settings.TestSceneData.SceneAsset);
                return;
            }
#endif

            // Otherwise, load the first listed scene
            if (_settings.SceneDatas != null && _settings.SceneDatas.Count > 0 && _settings.SceneDatas[0].SceneAsset != null)
            {
                Load(_settings.SceneDatas[0].SceneAsset);
            }

            Debug.Log($"[SceneService] Initialized with base scene: {baseSceneName}");
        }

        // Loads a scene additively on top of the base scene
        public static SceneData Load(SceneAsset sceneAsset, Action onLoaded = null)
        {
            if (!_isInitialized)
            {
                Debug.LogError("[SceneService] Cannot load scene before Initialize() is called.");
                return null;
            }

            if (sceneAsset == null)
            {
                Debug.LogWarning("[SceneService] Load() called with a null scene reference.");
                return null;
            }

            string sceneName = sceneAsset.name;

            if (_loadedSceneNames.Contains(sceneName))
            {
                Debug.LogWarning($"[SceneService] '{sceneName}' is already loaded.");
                onLoaded?.Invoke();
                return null;
            }

            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            op.completed += _ =>
            {
                var loadedScene = SceneManager.GetSceneByName(sceneName);
                _loadedSceneNames.Add(sceneName);
                SceneManager.SetActiveScene(loadedScene);

                OnSceneLoaded?.Invoke(sceneName);
                onLoaded?.Invoke();

                Debug.Log($"[SceneService] Loaded scene: {sceneName}");
            };

            return _settings.GetSceneDataByName(sceneName);
        }

        // Unloads a scene if its currently loaded
        public static void Unload(SceneAsset sceneAsset, Action onUnloaded = null)
        {
            if (sceneAsset == null)
            {
                Debug.LogWarning("[SceneService] Unload() called with a null scene reference.");
                onUnloaded?.Invoke();
                return;
            }

            string sceneName = sceneAsset.name;

            if (!_loadedSceneNames.Contains(sceneName))
            {
                Debug.LogWarning($"[SceneService] '{sceneName}' is not currently loaded.");
                onUnloaded?.Invoke();
                return;
            }

            var op = SceneManager.UnloadSceneAsync(sceneName);
            op.completed += _ =>
            {
                _loadedSceneNames.Remove(sceneName);
                OnSceneUnloaded?.Invoke(sceneName);
                onUnloaded?.Invoke();

                Debug.Log($"[SceneService] Unloaded scene: {sceneName}");
            };
        }

        // Closes all additive scenes, leaving only the base scene active
        public static void UnloadAllAdditives()
        {
            foreach (string sceneName in new List<string>(_loadedSceneNames))
            {
                if (_settings.BaseSceneData.SceneAsset != null && sceneName == _settings.BaseSceneData.SceneName)
                    continue;

                var op = SceneManager.UnloadSceneAsync(sceneName);
                op.completed += _ =>
                {
                    _loadedSceneNames.Remove(sceneName);
                    OnSceneUnloaded?.Invoke(sceneName);
                    Debug.Log($"[SceneService] Unloaded additive scene: {sceneName}");
                };
            }
        }

        // Checks if a specific scene is currently loaded in memory
        public static bool IsSceneLoaded(SceneAsset sceneAsset)
        {
            if (sceneAsset == null)
                return false;

            string name = sceneAsset.name;
            var scene = SceneManager.GetSceneByName(name);
            return scene.IsValid() && scene.isLoaded;
        }
    }
}
