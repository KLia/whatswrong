using System.Collections;
using System.Collections.Generic;
using System.IO;
using Game.Scripts.Game.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scripts
{
    public class SceneNavigation : MonoBehaviour
    {
        private const string RoomsFolder = "Assets/Game/Scenes/Rooms/";

        [SerializeField] private string startSceneName;

        private readonly List<string> _contentScenes = new List<string>();
        private int _currentIndex = -1;

        private bool _isSwitching = false;
        private SceneLoader _sceneLoader;
        private InputMapping _input;

        private void Awake()
        {
            _sceneLoader = FindAnyObjectByType<SceneLoader>(FindObjectsInactive.Include);
            _input = FindAnyObjectByType<InputMapping>();

            if (_input != null)
            {
                _input.NextRoom += ShowNext;
                _input.PreviousRoom += ShowPrevious;
            }

            BuildSceneList();
        }

        private void Start()
        {
            if (_contentScenes.Count == 0)
            {
                Debug.LogError("No content scenes found in Build Settings.");
                return;
            }

            ResolveStartSceneIndex();
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

        private void ResolveStartSceneIndex()
        {
            _currentIndex = 0;

            if (string.IsNullOrWhiteSpace(startSceneName))
                return;

            int index = _contentScenes.FindIndex(scenePath =>
                Path.GetFileNameWithoutExtension(scenePath) == startSceneName);

            if (index >= 0)
            {
                _currentIndex = index;
            }
            else
            {
                Debug.LogWarning($"Start scene '{startSceneName}' was not found in {_contentScenes.Count} room scenes. Falling back to first scene.");
            }
        }

        private IEnumerator LoadInitialScene()
        {
            _isSwitching = true;
            yield return _sceneLoader.LoadScene(_contentScenes[_currentIndex], false);
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

            yield return _sceneLoader.UnloadScene(currentScene);
            yield return _sceneLoader.LoadScene(nextScene);

            _currentIndex = nextIndex;
            _isSwitching = false;
        }

        private void OnDestroy()
        {
            if (_input != null)
            {
                _input.NextRoom -= ShowNext;
                _input.PreviousRoom -= ShowPrevious;
            }
        }
    }
}