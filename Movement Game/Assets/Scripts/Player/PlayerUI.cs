using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("Settings")]
    public float fadeDuration;

    [Header("UI Elements")]
    public RawImage hpBar;
    public RawImage o2Bar;
    public RawImage dmgBar;
    public TextMeshProUGUI interactText;

    [Header("References")]
    public PlayerManager pm;

    private void Start()
    {
        pm = GetComponent<PlayerManager>();
    }

    private void Update()
    {
        // hp bar
        float maxHPScale = pm.maxHP / pm.TotalHP;
        float hpScale = Mathf.Clamp(pm.hp / pm.maxHP, 0f, 1f);

        hpBar.transform.localScale = new Vector3(100f * hpScale * maxHPScale,
            hpBar.transform.localScale.y, hpBar.transform.localScale.z);

        // o2 bar
        float maxO2Scale = pm.maxHP / pm.TotalHP;
        float o2Scale = Mathf.Clamp(pm.o2 / pm.maxHP, 0f, 1f);

        o2Bar.transform.localScale = new Vector3(100f * o2Scale * maxO2Scale,
            o2Bar.transform.localScale.y, o2Bar.transform.localScale.z);

        // dmg bar
        float lostHPScale = (pm.TotalHP - pm.maxHP) / pm.TotalHP;

        dmgBar.transform.localScale = new Vector3(-100f * lostHPScale,
            dmgBar.transform.localScale.y, dmgBar.transform.localScale.z);


    }

    public void FadeOutInteractUI()
    {
        StopAllCoroutines();
        StartCoroutine(LerpUI_TMPro(interactText, 0));
    }    
    
    public void FadeInInteractUI()
    {
        StopAllCoroutines();
        StartCoroutine(LerpUI_TMPro(interactText, 1));
    }

    public void ChangeText(string input, float delay)
    {
        StartCoroutine(AlterText(input, delay));
    }

    IEnumerator AlterText(string input, float delay)
    {
        yield return new WaitForSeconds(delay);

        interactText.text = input;
    }
    IEnumerator LerpUI_TMPro(TextMeshProUGUI ui, float endValue)
    {
        float startAlpha = ui.color.a;
        float duration = fadeDuration;
        float elapsed = 0f;

        Color color = ui.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);  // Ensures t stays within [0, 1]
            color.a = Mathf.Lerp(startAlpha, endValue, t);
            ui.color = color;
            yield return null;
        }

        color.a = endValue;
        ui.color = color;
    }
}
