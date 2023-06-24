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
        // else if(col.tag == "Boss")
        // {
        //     AIAgent colBoss = col.GetComponentInParent<AIAgent>();
        //     Debug.Log(colBoss.name);
        //     if(colBoss.currentStateType != AIAgent.StateType.chasing)
        //     {
        //         return;
        //     }
        //     else
        //      {
        //        colBoss.stunParticles.Play();
        //          StartCoroutine(colBoss.chasingState.BossSlow(0.8f));
        //     
        //         DestroyBullet();
        //      }
        // }
    }
}
