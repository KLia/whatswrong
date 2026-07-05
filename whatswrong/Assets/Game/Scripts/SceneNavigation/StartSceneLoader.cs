using Game.Scripts.Game.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneLoader : MonoBehaviour
{
    [SerializeField] private string startSceneName;
    private SceneLoader _sceneLoader;


    private void Awake()
    {
        _sceneLoader = FindAnyObjectByType<SceneLoader>(FindObjectsInactive.Include);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneManager.LoadScene(startSceneName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
