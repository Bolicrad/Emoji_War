using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Audio : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource source;
    public AudioClip[] clips;

    void PlaySound(int id)
    {
        source.Stop();
        source.clip = clips[id % 3];
        source.Play();
    }
}
