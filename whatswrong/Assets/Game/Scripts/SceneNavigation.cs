using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Game.Scripts;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SceneNavigation : MonoBehaviour
{
    [SerializeField] private InputMapping input;
    [SerializeField] private SceneTransition sceneTransition;

    private List<string> contentScenes = new List<string>();
    private int currentIndex = -1;
    private bool isSwitching = false;

    private void Awake()
    {
        input.NextRoom += ShowNext;
        input.PreviousRoom += ShowPrevious;
        BuildSceneList();
    }

    private void Start()
    {
        if (contentScenes.Count == 0)
        {
            Debug.LogError("No content scenes found in Build Settings.");
            return;
        }

        StartCoroutine(LoadInitialScene());
    }

    void BuildSceneList()
    {
        string mainSceneName = SceneManager.GetActiveScene().name;

        int count = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < count; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = Path.GetFileNameWithoutExtension(path);

            if (sceneName == mainSceneName)
                continue; // skip main scene

            contentScenes.Add(sceneName);
        }
    }

    private IEnumerator LoadInitialScene()
    {
        isSwitching = true;
        yield return sceneTransition.FadeIn(contentScenes[0]);
        currentIndex = 0;
        isSwitching = false;
    }

    public void ShowNext()
    {
        if (!isSwitching)
            StartCoroutine(SwitchScene(+1));
    }

    public void ShowPrevious()
    {
        if (!isSwitching)
            StartCoroutine(SwitchScene(-1));
    }

    private IEnumerator SwitchScene(int direction)
    {
        isSwitching = true;

        int nextIndex = currentIndex + direction;

        if (nextIndex < 0)
            nextIndex = contentScenes.Count - 1;
        else if (nextIndex >= contentScenes.Count)
            nextIndex = 0;

        string currentScene = contentScenes[currentIndex];
        string nextScene = contentScenes[nextIndex];

        yield return sceneTransition.FadeOut(currentScene);
        yield return sceneTransition.FadeIn(nextScene);

        currentIndex = nextIndex;

        isSwitching = false;
    }

    private void OnDestroy()
    {
        input.NextRoom -= ShowNext;
        input.PreviousRoom -= ShowPrevious;
    }
}