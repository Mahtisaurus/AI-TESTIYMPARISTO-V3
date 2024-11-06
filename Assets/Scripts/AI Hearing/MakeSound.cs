using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeSound : MonoBehaviour
{
    [SerializeField] private AudioSource source = null;

    [SerializeField] private float soundRange = 25f;

    [SerializeField] private Sound.SoundType soundType = Sound.SoundType.Dangerous;

    private Sound sound;

    public void Start()
    {
        source = GetComponent<AudioSource>();
        sound = new Sound(transform.position, soundRange, soundType);
    }

    public void PlaySound()
    {
        source.Play();
        Sounds.MakeSound(sound);
    }
}