using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager _instance;
    AudioSource _source;

    public delegate void HasBackgroundMusic(bool hasBackgroundMusic);
    public event HasBackgroundMusic hasBackgroundMusic;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(this);

        _source = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_source.clip != null)
        {
            _source.volume = 0.1f;
            _source.Play();
        }
        else
        {
            if (hasBackgroundMusic != null)
                hasBackgroundMusic(false);
        }
    }
}
