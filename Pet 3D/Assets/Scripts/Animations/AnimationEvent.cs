using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    public delegate void OnAnimationEnd(string animationName);
    public event OnAnimationEnd onAnimationEnd;

    public void AnimationEnd(string animationName)
    {
        onAnimationEnd(animationName);
        Debug.Log("Called onAnimationEnd(" + animationName + ")");
    }
}
