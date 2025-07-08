using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class health_bar : MonoBehaviour
{
    public Slider health_slider;
    public Slider ease_health_slider;
    private float lerp_speed = 0.05f;
    private Coroutine easeRoutine;

    public void SetMaxHealth(float maxHealth)
    {
        health_slider.maxValue = maxHealth;
        ease_health_slider.maxValue = maxHealth;
    }

    public void UpdateHealthBar(float currentHealth)
    {
        health_slider.value = currentHealth;

        if (easeRoutine != null)
        {
            StopCoroutine(easeRoutine);
        }
        easeRoutine = StartCoroutine(UpdateEaseHealthBar(currentHealth));
    }

    private IEnumerator UpdateEaseHealthBar(float targetHealth)
    {
        while (Mathf.Abs(ease_health_slider.value - targetHealth) > 0.01f)
        {
            ease_health_slider.value = Mathf.Lerp(ease_health_slider.value, targetHealth,
                                                    lerp_speed * Time.deltaTime * 60f);
            yield return null;
        }

        ease_health_slider.value = targetHealth;
    }
}
