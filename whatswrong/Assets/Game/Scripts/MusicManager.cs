using UnityEngine;

[DisallowMultipleComponent]
public class MusicManager : MonoBehaviour
{
    private static MusicManager _instance;

    [SerializeField] private AudioClip musicClip;
    [SerializeField] [Range(0f, 1f)] private float volume = 0.5f;

    private AudioSource _audioSource;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.clip = musicClip;
        _audioSource.loop = true;
        _audioSource.volume = volume;
        _audioSource.playOnAwake = false;
        _audioSource.Play();
    }
}
