using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        StealthPlayerController player = StealthPlayerController.getInstance();
        if (other.gameObject == player.gameObject)
        {
            player.SetEnergy(0);
        }
    }
}
