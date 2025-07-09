using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    private RectTransform RT;
    private Vector2 original_anchor_pos;
    private float shake_duration;
    private float shake_power;

    private void Awake()
    {
        RT = GetComponent<RectTransform>();
        original_anchor_pos = RT.anchoredPosition;
    }

    public void Shake(float duration, float power)
    {
        shake_duration = duration;
        shake_power = power;
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine());
    }

    private System.Collections.IEnumerator ShakeRoutine()
    {
        float timer = 0f;

        while(timer < shake_duration)
        {
            Vector2 offset = Random.insideUnitCircle * shake_power;
            RT.anchoredPosition = original_anchor_pos + offset;

            timer += Time.deltaTime;
            yield return null;
        }

        RT.anchoredPosition = original_anchor_pos;
    }
}
