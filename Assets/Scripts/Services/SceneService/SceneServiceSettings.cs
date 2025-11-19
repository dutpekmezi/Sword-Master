using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dutpekmezi.Services
{
    [CreateAssetMenu(
        fileName = "SceneServiceSettings",
        menuName = "Game/Scriptable Objects/Services/SceneServiceSettings")]
    public class SceneServiceSettings : ScriptableObject
    {
        [Header("Base Scene")]
        public SceneData BaseSceneData;

        [Header("Game Scenes")]
        public List<SceneData> SceneDatas = new();

        [Header("Test Mode")]
        public bool TestMode = false;
        public SceneData TestSceneData;

        public string GetBaseSceneName() => BaseSceneData?.SceneName;

        public IEnumerable<string> GetAllSceneNames()
        {
            foreach (var scene in SceneDatas)
                if (scene != null)
                    yield return scene.SceneName;
        }

        public string GetTestSceneName() =>
            TestMode && TestSceneData != null
                ? TestSceneData.SceneName
                : null;

        public string GetSceneNameByType(SceneType type)
        {
            if (BaseSceneData != null && BaseSceneData.SceneType == type)
                return BaseSceneData.SceneName;

            foreach (var data in SceneDatas)
                if (data != null && data.SceneType == type)
                    return data.SceneName;

            if (TestSceneData != null && TestSceneData.SceneType == type)
                return TestSceneData.SceneName;

            return null;
        }

#if UNITY_EDITOR
        public SceneData GetSceneDataByType(SceneType type)
        {
            if (BaseSceneData != null && BaseSceneData.SceneType == type)
                return BaseSceneData;

            foreach (var data in SceneDatas)
                if (data != null && data.SceneType == type)
                    return data;

            if (TestSceneData != null && TestSceneData.SceneType == type)
                return TestSceneData;

            return null;
        }

        public SceneData GetSceneDataByName(string name)
        {
            if (BaseSceneData != null && BaseSceneData.SceneName == name)
                return BaseSceneData;

            foreach (var data in SceneDatas)
                if (data != null && data.SceneName == name)
                    return data;

            if (TestSceneData != null && TestSceneData.SceneName == name)
                return TestSceneData;

            return null;
        }
#endif
    }
}
