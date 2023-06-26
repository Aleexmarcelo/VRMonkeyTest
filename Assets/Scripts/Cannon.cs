using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cannon : MonoBehaviour
{
    [SerializeField]
    private Image energyBar;
    [SerializeField]
    private int maxEnergy;
    [SerializeField]
    private GameObject ballCannonPrefab;
    [SerializeField]
    private GameObject startPos;
    [SerializeField]
    private float trajectoryDelta;

    private float energy;

    private void Start()
    {
        energy = maxEnergy;
        UpdateBar();
    }

    public void Charge()
    {
        energy += Time.deltaTime;
        UpdateBar();
    }

    private void UpdateBar()
    {
        var energyValue = Remap(energy, 0, maxEnergy, 0, 1);
        energyBar.fillAmount = energyValue;

    }
    
    public float Remap (float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public bool CanShoot()
    {
        if (energy >= maxEnergy)
        {
            return true; 
        }

        return false;
    }

    public void Shoot()
    {
        energy = 0;
        UpdateBar();
        var ballCannon = Instantiate(ballCannonPrefab, startPos.transform.position, Quaternion.identity, null).GetComponent<BallCannon>();
        
        StartCoroutine(BallCannonTrajectory(ballCannon.transform));

    }
    
    private IEnumerator BallCannonTrajectory(Transform ballCannon)
    {
        var startPos = this.startPos.transform.position;
        var endPos = startPos + (this.startPos.transform.forward * 100);
        var alpha = 0f;

        while (alpha < 1f)
        {
            if (ballCannon == null)
            {
                yield break;
            }
            alpha += Time.deltaTime / trajectoryDelta;
            ballCannon.position = Vector3.Lerp(startPos, endPos, alpha);
            yield return null;
        }
    }
}
