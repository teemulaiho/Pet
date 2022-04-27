using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    AudioManager _audioManager;

    List<Animator> animators;
    List<AnimationEvent> animationEvents;

    Animator crossfade;
    Animator intro;

    private void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>();

        animators = new List<Animator>();
        animators.AddRange(FindObjectsOfType<Animator>());

        animationEvents = new List<AnimationEvent>();
        animationEvents.AddRange(FindObjectsOfType<AnimationEvent>());

        string animName;

        foreach (var anim in animators)
        {
            animName = anim.gameObject.name;

            if (animName.Contains("Intro"))
                intro = anim;
            else if (animName.Contains("Crossfade"))
                crossfade = anim;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var animEvent in animationEvents)
        {
            animEvent.onAnimationEnd += OnAnimationEnd;
        }

        if (_audioManager)
            _audioManager.hasBackgroundMusic += HasBackgroundMusic;

        StartAnimationWithDelay("PlayIntro", 0f);
    }

    void OnAnimationEnd(string animationName)
    {
        Debug.Log("TitleSceneManager Received onAnimationEnd(" + animationName + ")");

        if (animationName.Contains("Crossfade End"))
        {
            //StartAnimationWithDelay("PlayIntro", 0f);
        }
        else if (animationName.Contains("Intro End"))
        {
            StartAnimationWithDelay("PlayCrossfadeStart", 2f);
        }
        else if (animationName.Contains("Crossfade Start"))
            LoadScene("HomeScene", 1f);
    }

    void StartAnimationWithDelay(string animationToPlay, float secondsToWait)
    {
        Invoke(animationToPlay, secondsToWait);
    }

    void PlayIntro()
    {
        intro.SetTrigger("PlayIntro");
    }

    void PlayCrossfadeStart()
    {
        crossfade.SetTrigger("Start");
    }

    void LoadScene(string sceneToLoad, float waitTime)
    {
        if (waitTime > 0f)
            StartCoroutine(LoadSceneInWithDelay(sceneToLoad, 2f));
        else
            SceneManager.LoadScene(sceneToLoad);
    }

    IEnumerator LoadSceneInWithDelay(string sceneToLoad, float secondsToWait)
    {
        yield return new WaitForSecondsRealtime(secondsToWait);
        SceneManager.LoadScene(sceneToLoad);
        yield return null;
    }

    void HasBackgroundMusic(bool hasBackgroundMusic)
    {
        if (!hasBackgroundMusic)
            intro.speed = 2f;
        else
            intro.speed = 1f;
    }
}
