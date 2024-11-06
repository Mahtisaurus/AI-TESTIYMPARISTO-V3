using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    PlayerMovementAdvanced playerMove;
    Rigidbody rb;

    [SerializeField] private AudioSource source = null;

    [SerializeField] private float soundRangeSprint = 10f;
    [SerializeField] private float soundRangeWalk = 5f;
    [SerializeField] private float soundRange = 0.5f;

    [SerializeField] private Sound.SoundType soundType = Sound.SoundType.Dangerous;

    private Sound sound;

    private float timer = 0f;
    private float interval = 0.7f;


    public void Start()
    {
        playerMove = GetComponent<PlayerMovementAdvanced>();
        rb = GetComponent<Rigidbody>();
        source = GetComponent<AudioSource>();
        sound = new Sound(transform.position, soundRange, soundType);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            SoundBasedOnState();
            PlaySound();

            timer = 0f;
        }
    }

    private void SoundBasedOnState()
    {
        if (playerMove.moveState == PlayerMovementAdvanced.MovementState.SPRINTING && rb.velocity.magnitude > 0.1f)
        {
            sound.pos = transform.position;
            sound.range = soundRangeSprint;
        }
        else if(playerMove.moveState == PlayerMovementAdvanced.MovementState.WALKING && rb.velocity.magnitude > 0.1f)
        {
            sound.pos = transform.position;
            sound.range = soundRangeWalk;
        }
        // Crouching
        else
        {
            sound.pos = transform.position;
            sound.range = soundRange;
        }

    }

    public void PlaySound()
    {
        // Use this function to play the sound. Now we just use the virtual functionality of creating a sound that the enemy AI can hear
        //source.Play();
        Sounds.MakeSound(sound);
    }
}
