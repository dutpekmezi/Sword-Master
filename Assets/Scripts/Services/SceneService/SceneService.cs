using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dutpekmezi.Services
{
    public class SceneService
    {
        private static SceneServiceSettings _settings;
        private static readonly HashSet<string> _loadedScenes = new();
        private static bool _initialized;

        public static event Action<SceneData> OnSceneLoaded;
        public static event Action<SceneData> OnSceneUnloaded;


        public SceneService(SceneServiceSettings settings)
        {
            Initialize(settings);
        }


        private static void Initialize(SceneServiceSettings settings)
        {
            if (_initialized)
                return;

            if (settings == null)
            {
                Debug.LogError("[SceneService] SceneServiceSettings is missing!");
                return;
            }

            _settings = settings;
            _initialized = true;

            var baseScene = settings.GetBaseScene();

            if (baseScene == null)
            {
                Debug.LogError("[SceneService] BaseScene is not set!");
                return;
            }

            SceneManager.LoadScene(baseScene.SceneName, LoadSceneMode.Single);
            _loadedScenes.Clear();
            _loadedScenes.Add(baseScene.SceneName);

            Debug.Log($"[SceneService] Base scene loaded: {baseScene.SceneName}");

            if (settings.TestMode && settings.TestScene != null)
            {
                Load(settings.TestScene);
                return;
            }

            if (settings.Scenes.Count > 0 && settings.Scenes[0] != null)
                Load(settings.Scenes[0]);
        }

        public static SceneData Load(SceneData data, Action onLoaded = null)
        {
            if (!_initialized)
            {
                Debug.LogError("[SceneService] Not initialized!");
                return null;
            }

            if (data == null)
            {
                Debug.LogError("[SceneService] Load called with null SceneData!");
                return null;
            }

            string name = data.SceneName;

            if (_loadedScenes.Contains(name))
            {
                Debug.LogWarning($"[SceneService] Scene already loaded: {name}");
                onLoaded?.Invoke();
                return data;
            }

            var op = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);

            op.completed += _ =>
            {
                _loadedScenes.Add(name);
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));

                OnSceneLoaded?.Invoke(data);
                onLoaded?.Invoke();

                Debug.Log($"[SceneService] Loaded scene: {name}");
            };

            return data;
        }

        public static void Unload(SceneData data, Action onUnloaded = null)
        {
            if (data == null)
            {
                Debug.LogError("[SceneService] Unload called with null SceneData!");
                onUnloaded?.Invoke();
                return;
            }

            string name = data.SceneName;

            if (!_loadedScenes.Contains(name))
            {
                Debug.LogWarning($"[SceneService] Scene not loaded: {name}");
                onUnloaded?.Invoke();
                return;
            }

            var op = SceneManager.UnloadSceneAsync(name);

            op.completed += _ =>
            {
                _loadedScenes.Remove(name);
                OnSceneUnloaded?.Invoke(data);
                onUnloaded?.Invoke();

                Debug.Log($"[SceneService] Unloaded: {name}");
            };
        }

        public static void UnloadAllAdditives()
        {
            var baseScene = _settings.GetBaseScene();

            foreach (var sceneName in new List<string>(_loadedScenes))
            {
                if (sceneName == baseScene.SceneName)
                    continue;

                var data = _settings.GetSceneByName(sceneName);

                Unload(data);
            }
        }

        public static bool IsLoaded(SceneData data)
        {
            if (data == null)
                return false;

            return _loadedScenes.Contains(data.SceneName);
        }
    }
}
