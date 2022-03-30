using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    List<Animator> animators;
    List<AnimationEvent> animationEvents;

    Animator crossfade;
    Animator intro;

    private void Awake()
    {
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
        foreach (var aevent in animationEvents)
        {
            aevent.onAnimationEnd += OnAnimationEnd;
        }
    }

    void OnAnimationEnd(string animationName)
    {
        Debug.Log("TitleSceneManager Received onAnimationEnd(" + animationName + ")");

        if (animationName.Contains("Crossfade End"))
            intro.SetTrigger("PlayIntro");
        else if (animationName.Contains("Intro End"))
            crossfade.SetTrigger("Start");
        else if (animationName.Contains("Crossfade Start"))
            LoadScene("HomeScene", 1f);

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
}
