using System.Collections;
using System.Linq;

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
                CleanUpScene();
                yield return _sceneTransition.FadeOut(sceneName);
            }

            private static void CleanUpScene()
            {
                foreach (var handler in FindObjectsOfType<MonoBehaviour>().OfType<ISceneUnlaodHandler>())
                {
                    handler.OnBeforeSceneUnload();
                }
            }

            public IEnumerator LoadScene(string sceneName, bool fade = true)
            {
                foreach (var handler in FindObjectsOfType<MonoBehaviour>().OfType<ISceneLoadHandler>())
                {
                    handler.OnBeforeSceneLoad();
                }

                if (fade)
                {
                    yield return _sceneTransition.FadeIn(sceneName);
                }
                else
                {
                    yield return SceneManager.LoadSceneAsync(
                        sceneName,
                        LoadSceneMode.Additive
                    );
                }
            }
            

            private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
            {
                if (mode == LoadSceneMode.Additive)
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