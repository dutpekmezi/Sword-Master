using System.Collections.Generic;
using UnityEngine;

namespace Dutpekmezi.Services
{
    [CreateAssetMenu(
        fileName = "SceneServiceSettings",
        menuName = "Game/Scriptable Objects/Services/SceneServiceSettings")]
    public class SceneServiceSettings : ScriptableObject
    {
        [Header("Base Scene")]
        public SceneData BaseScene;

        [Header("Game Scenes")]
        public List<SceneData> Scenes = new();

        [Header("Test Mode")]
        public bool TestMode = false;
        public SceneData TestScene;

        public SceneData GetBaseScene() => BaseScene;

        public SceneData GetSceneByType(SceneType type)
        {
            if (BaseScene != null && BaseScene.SceneType == type)
                return BaseScene;

            foreach (var s in Scenes)
                if (s.SceneType == type)
                    return s;

            if (TestMode && TestScene != null && TestScene.SceneType == type)
                return TestScene;

            return null;
        }

        public SceneData GetSceneByName(string sceneName)
        {
            if (BaseScene != null && BaseScene.SceneName == sceneName)
                return BaseScene;

            foreach (var s in Scenes)
                if (s.SceneName == sceneName)
                    return s;

            if (TestMode && TestScene != null && TestScene.SceneName == sceneName)
                return TestScene;

            return null;
        }
    }
}
