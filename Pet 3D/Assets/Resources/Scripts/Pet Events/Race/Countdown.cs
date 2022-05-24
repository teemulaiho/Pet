using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    private AudioSource source;

    [SerializeField] AudioClip horn;
    [SerializeField] AudioClip gun;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlayHorn()
    {
        source.PlayOneShot(horn);
    }

    public void PlayGun()
    {
        source.PlayOneShot(gun);
    }
}
