using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager _instance;

    AudioSource _source;

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
        _source.Play();
        _source.volume = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
