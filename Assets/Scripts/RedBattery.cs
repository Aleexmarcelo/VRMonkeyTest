using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBattery : MonoBehaviour
{
    [SerializeField]
    private Material redBatteryMaterial;
    [SerializeField]
    private Color greenBatteryColor;
    [SerializeField]
    private Renderer renderer;
    

    public bool isRedBattery = true;

    public Boss boss;

    public void ConvertBattery()
    {
        renderer.material = redBatteryMaterial;
        isRedBattery = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = StealthPlayerController.getInstance();
        if (player.gameObject == other.gameObject)
        {
            if (isRedBattery)
            {
                player.SetEnergy(0);
            }
            else
            {
                player.AddEnergy(player.maxEnergy);
            }
            
            Destroy(gameObject);
        }
    }
}
