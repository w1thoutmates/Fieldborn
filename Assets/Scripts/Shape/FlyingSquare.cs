using UnityEngine;
using UnityEngine.UI;

public class FlyingSquare : MonoBehaviour
{
    public Image image;
    public float fallDistance = 30f;
    public float fallDuration = 0.3f; 
    public float suckSpeed = 5f; 
    public float scaleSpeed = 0.5f; 

    private Vector3 targetPosition;
    private Vector3 startPosition;
    private float timer;
    private bool isFalling = true;

    public void Init(Color color, Vector3 target)
    {
        image.color = color;
        targetPosition = target;
        startPosition = transform.position;
        Destroy(gameObject, 3f);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (isFalling)
        {
            float progress = Mathf.Clamp01(timer / fallDuration);
            transform.position = Vector3.Lerp(
                startPosition,
                startPosition + Vector3.down * fallDistance,
                progress
            );

            if (progress >= 1f)
            {
                isFalling = false;
                timer = 0f;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                suckSpeed * Time.deltaTime
            );

            transform.localScale -= Vector3.one * scaleSpeed * Time.deltaTime;
        }
    }
}
