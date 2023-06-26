using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public Image healthBar;
    [SerializeField]
    private GameObject shootPrefab;
    [SerializeField]
    private GameObject superBatteryPrefab;
    [SerializeField]
    private float orbitSpeed = 5;
    [SerializeField]
    private float lookSpeed = 5;
    [SerializeField]
    private float shootDelay = 5;
    [SerializeField]
    private Transform center;
    [SerializeField]
    private Transform shootPoint;
    [SerializeField]
    private Transform body;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private float trajectoryDelta;
    [SerializeField]
    private float maxHealth;
    [SerializeField]
    private float health;

    public List<GameObject> redBatteries = new List<GameObject>();

    public bool inArea;
    public bool alive = true;

    private IEnumerator Start()
    {
        health = maxHealth;
        
        while (true)
        {
            if (inArea && alive)
            {
                yield return new WaitForSeconds(shootDelay);

                var redBattery = Instantiate(shootPrefab, shootPoint.transform);

                redBattery.transform.parent = null;

                redBatteries.Add(redBattery);

                redBattery.GetComponent<RedBattery>().boss = this;
            
                StartCoroutine(BatteryTrajectory(redBattery.transform));
            }
            else
            {
                yield return null;
            }
        }
    }

    private void Update()
    {
        body.rotation *= Quaternion.Euler(Vector3.up * (lookSpeed * Time.deltaTime));
        center.rotation *= Quaternion.Euler(Vector3.up * (orbitSpeed * Time.deltaTime));
    }

    private IEnumerator BatteryTrajectory(Transform redBattery)
    {
        var startPos = shootPoint.transform.position;
        var endPos = playerTransform.position;
        var alpha = 0f;

        while (alpha < 1f)
        {
            if (redBattery == null)
            {
                yield break;
            }
            alpha += Time.deltaTime / trajectoryDelta;
            redBattery.position = Vector3.Lerp(startPos, endPos, alpha);
            yield return null;
        }
    }

    public void TakeDamage()
    {
        health -= 51;
        healthBar.fillAmount = Remap(health, 0, maxHealth, 0, 1);
        if (health <= 0)
        {
            body.gameObject.SetActive(false);
            alive = false;
            
            var redBattery = Instantiate(superBatteryPrefab, shootPoint.transform);

            redBattery.transform.parent = null;

            redBatteries.Add(redBattery);

            StartCoroutine(BatteryTrajectory(redBattery.transform));
        }
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public void ResetBoss()
    {
        body.gameObject.SetActive(true);
        health = maxHealth;
        healthBar.fillAmount = Remap(health, 0, maxHealth, 0, 1);
        alive = true;
    }
}
