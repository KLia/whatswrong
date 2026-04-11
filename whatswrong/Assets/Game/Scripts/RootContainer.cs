using Game.Scripts;
using UnityEngine;

public class RootContainer : MonoBehaviour
{
    private InputMapping _input;
    private SceneNavigation _sceneNavigation;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _input = GetComponent<InputMapping>();
        _sceneNavigation = GetComponent<SceneNavigation>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
