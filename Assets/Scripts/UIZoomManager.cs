using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIZoomManager : MonoBehaviour
{
    public static UIZoomManager instance;
    public RectTransform uiRoot;

    private Vector3 normalScale;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        if (uiRoot != null)
            normalScale = uiRoot.localScale;
    }

    public System.Collections.IEnumerator Zoom(float zoomMultiplier = 1.2f, float zoomDuration = 0.3f)
    {
        if (uiRoot == null) yield break;

        Vector3 target = normalScale * zoomMultiplier;

        yield return uiRoot.DOScale(target, zoomDuration).WaitForCompletion();

        uiRoot.localScale = normalScale;
    }
}
