using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Static class for calling MakeSound
public static class Sounds
{
    public static void MakeSound(Sound sound)
    {
        Collider[] col = Physics.OverlapSphere(sound.pos, sound.range);

        for (int i = 0; i < col.Length; i++)
        {
            if (col[i].TryGetComponent(out IHear hearer))
            {
                // AIAlertSystem
                hearer.RespondToSound(sound);
            }
        }
    }
}