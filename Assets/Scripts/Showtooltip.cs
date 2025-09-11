using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Showtooltip : MonoBehaviour
{
    public static Showtooltip instance;
    public GameObject description_window_prefab;
    private GameObject current_tooltip;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }

    public void ShowTooltip(Item item, Vector3 position)
    {
        if (current_tooltip != null) Destroy(current_tooltip);

        Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        current_tooltip = Instantiate(description_window_prefab, canvas.transform);

        RectTransform rect = current_tooltip.GetComponent<RectTransform>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            position,
            canvas.worldCamera,
            out localPoint
        );

        Vector2 targetPosition = localPoint + Vector2.up * 225f;
        rect.anchoredPosition = targetPosition;

        Vector2 tooltipSize = rect.sizeDelta;

        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;
        Vector2 canvasMin = new Vector2(-canvasWidth / 2f, -canvasHeight / 2f);
        Vector2 canvasMax = new Vector2(canvasWidth / 2f, canvasHeight / 2f);

        Vector2 tooltipMin = targetPosition - rect.pivot * tooltipSize;
        Vector2 tooltipMax = tooltipMin + tooltipSize;

        Vector2 adjustedPosition = targetPosition;

        if (tooltipMax.x > canvasMax.x)
        {
            adjustedPosition.x -= tooltipMax.x - canvasMax.x + 10f; 
        }
        
        if (tooltipMin.x < canvasMin.x)
        {
            adjustedPosition.x += canvasMin.x - tooltipMin.x + 10f;
        }
        
        if (tooltipMax.y > canvasMax.y)
        {
            adjustedPosition.y -= tooltipMax.y - canvasMax.y + 10f;
        }
        
        if (tooltipMin.y < canvasMin.y)
        {
            adjustedPosition.y += canvasMin.y - tooltipMin.y + 10f;
        }

        rect.anchoredPosition = adjustedPosition;

        TMP_Text name_text = current_tooltip.transform.Find("item_name_text").GetComponent<TMP_Text>();
        TMP_Text desc_text = current_tooltip.transform.Find("item_description_text").GetComponent<TMP_Text>();
        TMP_Text type_text = current_tooltip.transform.Find("item_type_text").GetComponent<TMP_Text>();

        name_text.text = $"<color=#{ColorUtility.ToHtmlStringRGB(ItemQualityColors.GetColor(item.item_quality))}>{item.name}</color>";
        desc_text.text = item.description;
        type_text.text = item.item_type.ToString();

        foreach (var g in current_tooltip.GetComponentsInChildren<Graphic>())
        {
            g.raycastTarget = false;
        }
    }
    public void HideTooltip()
    {
        if (current_tooltip != null)
        {
            Destroy(current_tooltip);
            current_tooltip = null;
        }
    }

}
