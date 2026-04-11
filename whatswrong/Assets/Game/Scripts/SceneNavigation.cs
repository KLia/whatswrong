using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scripts
{
    public class SceneNavigation : MonoBehaviour
    {
        [SerializeField] private InputMapping input;
        [SerializeField] private SceneTransition sceneTransition;

        private const string RoomsFolder = "Assets/Game/Scenes/Rooms/";
        private readonly List<string> _contentScenes = new List<string>();
        private int _currentIndex = -1;
    
        private bool _isSwitching = false;

        private void Awake()
        {
            input.NextRoom += ShowNext;
            input.PreviousRoom += ShowPrevious;
            BuildSceneList();
        }

        private void Start()
        {
            if (_contentScenes.Count == 0)
            {
                Debug.LogError("No content scenes found in Build Settings.");
                return;
            }

            StartCoroutine(LoadInitialScene());
        }

        private void BuildSceneList()
        {
            _contentScenes.Clear();

            string mainScenePath = SceneManager.GetActiveScene().path;

            int count = SceneManager.sceneCountInBuildSettings;

            for (int i = 0; i < count; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);

                if (scenePath == mainScenePath)
                    continue;

                if (!scenePath.StartsWith(RoomsFolder))
                    continue;

                _contentScenes.Add(scenePath);
            }
        }
    
        private IEnumerator LoadInitialScene()
        {
            _isSwitching = true;
            yield return sceneTransition.FadeIn(_contentScenes[0]);
            _currentIndex = 0;
            _isSwitching = false;
        }

        private void ShowNext()
        {
            if (!_isSwitching)
                StartCoroutine(SwitchScene(+1));
        }

        private void ShowPrevious()
        {
            if (!_isSwitching)
                StartCoroutine(SwitchScene(-1));
        }

        private IEnumerator SwitchScene(int direction)
        {
            _isSwitching = true;

            var nextIndex = _currentIndex + direction;

            if (nextIndex < 0)
                nextIndex = _contentScenes.Count - 1;
            else if (nextIndex >= _contentScenes.Count)
                nextIndex = 0;

            var currentScene = _contentScenes[_currentIndex];
            var nextScene = _contentScenes[nextIndex];

            yield return sceneTransition.FadeOut(currentScene);
            yield return sceneTransition.FadeIn(nextScene);

            _currentIndex = nextIndex;
            _isSwitching = false;
        }

        private void OnDestroy()
        {
            input.NextRoom -= ShowNext;
            input.PreviousRoom -= ShowPrevious;
        }
    }
}