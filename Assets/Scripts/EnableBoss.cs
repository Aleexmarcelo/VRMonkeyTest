using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableBoss : MonoBehaviour
{
    [SerializeField]
    private Boss boss;

    private void Start()
    {
        boss.inArea = false;
        boss.healthBar.transform.parent.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = StealthPlayerController.getInstance();

        if (player.gameObject == other.gameObject)
        {
            boss.inArea = true;
            boss.healthBar.transform.parent.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var player = StealthPlayerController.getInstance();

        if (player.gameObject == other.gameObject)
        {
            boss.inArea = false;
            boss.healthBar.transform.parent.gameObject.SetActive(false);
        }
    }
}
