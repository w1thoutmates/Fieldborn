using UnityEngine;

public class SquareExplosionEffect : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystem.MainModule main;

    public void Play(Color color)
    {
        if (ps == null)
        {
            ps = GetComponent<ParticleSystem>();
            main = ps.main;
        }

        var color_over_lt = ps.colorOverLifetime;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        color_over_lt.color = grad;

        ps.Play();
    }
}
