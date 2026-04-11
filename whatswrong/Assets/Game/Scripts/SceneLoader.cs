using System.Collections;

namespace Game.Scripts
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    namespace Game.Scripts
    {
        public class SceneLoader : MonoBehaviour
        {
            private InputMapping _inputMapping;
            private SceneTransition _sceneTransition;


            private void Awake()
            {
                _inputMapping = FindFirstObjectByType<InputMapping>();
                _sceneTransition = FindFirstObjectByType<SceneTransition>();
            }

            private void OnEnable()
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
            }

            private void OnDisable()
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }

            public IEnumerator UnloadScene(string sceneName)
            {
                yield return _sceneTransition.FadeOut(sceneName);
            }
            
            public IEnumerator LoadScene(string sceneName)
            {
                yield return _sceneTransition.FadeIn(sceneName);
            }

            private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
            {
                if (mode != LoadSceneMode.Additive)
                    return;

                foreach (var root in scene.GetRootGameObjects())
                {
                    var controller = root.GetComponentInChildren<CameraController>(true);

                    if (controller != null)
                    {
                        controller.Initialize(_inputMapping);
                        return;
                    }
                }

                Debug.LogWarning("No CameraController found in loaded scene: " + scene.name);
            }
        }
    }
}