using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int slot_index;
    private InventoryUI parentUI;
    private Image icon;

    public void Init(InventoryUI ui, int index, Sprite sprite)
    {
        parentUI = ui;
        slot_index = index;

        icon = transform.Find("item_icon").GetComponent<Image>();
        icon.sprite = sprite;
        icon.color = sprite != null ? Color.white : new Color(1, 1, 1, 0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slot_index < 0 || slot_index >= parentUI.inventory.items.Length)
            return;

        var item = parentUI.inventory.items[slot_index];
        if (item != null)
        {
            RectTransform rect = GetComponent<RectTransform>();

            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("Canvas not found!");
                return;
            }

            Vector2 slot_ñenter = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rect.position);
            Vector2 local_point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                slot_ñenter,
                canvas.worldCamera,
                out local_point
            );

            Vector3 screen_position = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rect.TransformPoint(rect.rect.center));

            Showtooltip.instance.ShowTooltip(item, screen_position + Vector3.up * 35f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Showtooltip.instance.HideTooltip();
    }
}
