using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : DamageArea
{
    public override void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Enemy")
        {
            AIAgent colEnemy = col.GetComponent<AIAgent>();
            colEnemy.aiEnabled = false;
            colEnemy.searchLight.enabled = false;

            DestroyBullet();
        }
        
        var cannon = col.GetComponentInParent<Cannon>();

        if (cannon)
        {
            if (cannon.CanShoot())
            {
                cannon.Shoot();

                var player = StealthPlayerController.getInstance();
            
                player.AddEnergy(20);
            }
        }
    }
}
