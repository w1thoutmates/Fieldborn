using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public float move_speed = 1f;
    public float lifetime = 1f;
    public Vector3 offset = new Vector3(0.5f, 1f, 0);

    public AnimationCurve scale_curve;
    public AnimationCurve alpha_curve;

    private float timer;
    private TextMeshProUGUI text_mesh;
    private Color text_color;
    private Vector3 random_offset;

    private void Awake()
    {
        text_mesh = GetComponent<TextMeshProUGUI>();
        text_color = text_mesh.color;

        float angle = Random.Range(-15f, 15f);
        float radians = angle * Mathf.Deg2Rad;
        random_offset = new Vector3(Mathf.Sin(radians), Mathf.Cos(radians), 0);
    }

    public void Setup(int value, Color color, bool isCrit = false)
    {
        text_mesh.text = isCrit ? $"<size=110%>CRITICAL!</size>\n{value}" : value.ToString();
        text_mesh.color = color;
        text_color = color;
    }

    private void Update()
    {
        if (PauseManager.isPaused) return; 

        timer += Time.deltaTime;

        transform.position += random_offset * move_speed * Time.deltaTime;

        float t = timer / lifetime;
        transform.localScale = Vector3.one * scale_curve.Evaluate(t);
        text_mesh.color = new Color(text_color.r, text_color.g, text_color.b, alpha_curve.Evaluate(t));

        if(timer >= lifetime) Destroy(gameObject);
    }


}
