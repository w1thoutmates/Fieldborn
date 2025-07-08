using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker instance;

    private Vector3 original_position;
    private float shake_timer;
    private float shake_power;
    private float shake_fade_time;
    private float shake_rotation;

    public float rotation_multiplier = 15f;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        original_position = transform.position;
    }

    private void Update()
    {
        if (shake_timer > 0)
        {
            shake_timer -= Time.deltaTime;

            float xAmount = Random.Range(-1f, 1f) * shake_power;
            float yAmount = Random.Range(-1f, 1f) * shake_power;

            transform.position = original_position + new Vector3(xAmount, yAmount, 0f);

            shake_power = Mathf.MoveTowards(shake_power, 0f, shake_fade_time * Time.deltaTime);
        }
        else
        {
            transform.position = original_position;
        }
    }

    public void Shake(float duration = 0.3f, float magnitude = 0.2f)
    {
        original_position = transform.position;
        shake_timer = duration;
        shake_power = magnitude;
        shake_fade_time = magnitude / duration;

        Debug.Log("Camera трясется!");
    }
}
