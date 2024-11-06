using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSound : MonoBehaviour
{
    [SerializeField] private AudioSource source = null;

    [SerializeField] private float soundRange = 25f;

    [SerializeField] private Sound.SoundType soundType = Sound.SoundType.Dangerous;

    public void Start()
    {
        source = GetComponent<AudioSource>();
    }

    private void OnMouseDown()
    {
        if (source.isPlaying) //If already playing a sound, don't allow overlapping sounds 
            return;

        source.Play();

        var sound = new Sound(transform.position, soundRange, soundType);

        //Debug.Log($"Sound: with pos {sound.pos} and range {sound.range} created.");
        Sounds.MakeSound(sound);
    }
}
