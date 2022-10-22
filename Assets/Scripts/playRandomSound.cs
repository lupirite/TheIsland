using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playRandomSound : MonoBehaviour
{
    public AudioClip[] sounds;
    public float maxSize = 1;
    public bool changePitch;
    void Start()
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = sounds[Random.Range(0, sounds.Length)];
        audio.volume = transform.localScale.magnitude/(maxSize*2);
        if (changePitch) {
            audio.pitch = 1-transform.localScale.magnitude/(maxSize*2)+.5f;
        }
        audio.Play();
    }
}
