using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public Transform inv_slot_container;
    public GameObject inv_slot_prefab;
    public Sprite empty_slot_image;
    public bool isPlayer;

    public Button left_arrow;
    public Button right_arrow;

    private int current_index = 0;
    private const int VISIBLE_SLOTS = 3;

    private void Start()
    {
        left_arrow.onClick.AddListener(ScrollLeft);
        right_arrow.onClick.AddListener(ScrollRight);
        UpdateUI();
    }

    private void Update()
    {
        if (PauseManager.isPaused) return;
        if (!isPlayer) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            UsePlayerItem(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            UsePlayerItem(1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            UsePlayerItem(2);
    }

    public void ScrollLeft()
    {
        current_index = Mathf.Max(0, current_index - 1);
        UpdateUI();
    }

    public void ScrollRight()
    {
        if (inventory.items.Length <= VISIBLE_SLOTS) return;
        current_index = Mathf.Min(inventory.items.Length - VISIBLE_SLOTS, current_index + 1);
        UpdateUI();
    }

    public void UpdateUI()
    {
        foreach (Transform child in inv_slot_container)
            Destroy(child.gameObject);

        for (int i = 0; i < VISIBLE_SLOTS; i++)
        {
            int item_index = current_index + i;

            GameObject slotGO = Instantiate(inv_slot_prefab, inv_slot_container);

            Image slot_bg = slotGO.GetComponent<Image>();
            if (slot_bg != null)
                slot_bg.sprite = empty_slot_image;

            //Image item_icon = slotGO.transform.Find("item_icon")?.GetComponent<Image>();
            //if (item_icon == null)
            //{
            //    Debug.LogError("Префаб должен содержать Image с именем item_icon");
            //    continue;
            //}

            //if (item_index < inventory.items.Length && inventory.items[item_index] != null)
            //{
            //    item_icon.sprite = inventory.items[item_index].icon;
            //    item_icon.color = Color.white;
            //}
            //else
            //{
            //    item_icon.sprite = null;
            //    item_icon.color = new Color(1, 1, 1, 0);
            //}

            Sprite sprite = null;
            //bool isInteractable = false;

            if (item_index < inventory.items.Length && inventory.items[item_index] != null)
            {
                sprite = inventory.items[item_index].icon;
                //if (inventory.items[item_index] is ICooldownable cooldownable)
                //{
                //    isInteractable = cooldownable.isReady();
                //}
                //else
                //{
                //    isInteractable = inventory.items[item_index].item_type == ItemType.Active;
                //}
            }

            var slotUI = slotGO.AddComponent<ItemSlotUI>();
            slotUI.Init(this, item_index, sprite);
            //Button slotButton = slotGO.AddComponent<Button>();
            //Image slotImage = slotGO.GetComponent<Image>();
            //slotButton.targetGraphic = slotImage;
            //if (slotButton != null)
            //{
            //    int index = item_index;
            //    slotButton.onClick.AddListener(() => UsePlayerItem(index - current_index));
            //}
        }

        left_arrow.interactable = current_index > 0;
        right_arrow.interactable = current_index < Mathf.Max(0, inventory.items.Length - VISIBLE_SLOTS);
    }

    public void UsePlayerItem(int visible_index)
    {
        if (!TurnManager.instance.IsPlayerTurn()) return;

        int item_index = current_index + visible_index;
        if (item_index >= inventory.items.Length) return;

        Item item = inventory.items[item_index];

        if (inventory.items[item_index] == null) return;

        if (item.item_type == ItemType.Active)
        {
            if (item is ICooldownable cooldownable && !cooldownable.isReady()) return;

            item.ApplyToPlayer(Player.instance);
            Debug.Log($"Использован предмет: {item.name}");

            if (item.isDisposable)
            {
                inventory.items[item_index] = null;
            }

            UpdateUI();
        }
        else
        {
            Debug.Log($"Предмет {item.name} не активный");
        }
    }

    public void UseEnemyItem(int visible_index)
    {
        if (!TurnManager.instance.IsEnemyTurn()) return;

        int item_index = current_index + visible_index;
        if (item_index >= inventory.items.Length) return;

        Item item = inventory.items[item_index];

        if (inventory.items[item_index] == null) return;

        if (item.item_type == ItemType.Active)
        {
            Base_enemy enemy = FindObjectOfType<Base_enemy>();
            item.ApplyToEnemy(enemy);
            Debug.Log($"Враг использовал предмет: {item.name}");

            if (item.isDisposable)
            {
                inventory.items[item_index] = null;
            }

            UpdateUI();
        }
        else
        {
            Debug.Log($"Предмет {item.name} не активный");
        }
    }

}
