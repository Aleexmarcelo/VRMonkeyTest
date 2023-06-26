using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCannon : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        var boss = other.GetComponent<Boss>();

        if (boss)
        {
            boss.TakeDamage();
            Destroy(gameObject);
        }
    }
}
