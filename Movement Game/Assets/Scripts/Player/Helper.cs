using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
    public void RegenO2(float targetHP, float speed, PlayerManager pm)
    {
        StartCoroutine(StartRegenO2(targetHP, speed, pm));
    }

    IEnumerator StartRegenO2(float targetHP, float speed, PlayerManager pm)
    {
        targetHP = Mathf.Clamp(targetHP, 0f, pm.maxHP); // Ensure target HP is within bounds

        while (pm.o2 < targetHP)
        {
            pm.o2 = Mathf.Min(pm.o2 + speed * Time.deltaTime, targetHP);
            yield return null; // Wait for the next frame
            
        }
    }
}
