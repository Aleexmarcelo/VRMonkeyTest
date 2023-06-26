using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drain : MonoBehaviour
{
    private Cannon cannon;
    
    private void OnTriggerEnter(Collider other)
    {
        cannon = other.GetComponentInParent<Cannon>();
    }

    private void OnTriggerExit(Collider other)
    {
        cannon = other.GetComponentInParent<Cannon>();

        if (cannon)
        {
            cannon = null;
        }
    }

    private void Update()
    {
        var player = StealthPlayerController.getInstance();
        if (cannon && player.state == Character.States.attacking)
        {
            cannon.Charge();
            if (!cannon.CanShoot())
            {
                player.AddEnergy(0.1f);
            }
        }
    }
}
