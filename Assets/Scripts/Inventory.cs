using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Item[] items = new Item[8];

    private Player pl;
    private Base_enemy en;

    private void Awake()
    {
        pl = GetComponent<Player>();
        en = GetComponent<Base_enemy>();

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
            {
                items[i] = Instantiate(items[i]);
            }
        }
    }

    private void Start()
    {
        foreach (var item in items)
        {
            if (pl != null && item.item_type == ItemType.Passive)
            {
                item.ApplyToPlayer(pl);
            }
            else if(en != null && item.item_type == ItemType.Passive)
            {
                item.ApplyToEnemy(en);
            }
        }
    }

    public bool HasDisableClearBoardItem()
    {
        foreach (Item item in items)
        {
            if (item is Clear_board_disabler clearBoardDisabler && clearBoardDisabler.disable_clear_board)
                return true;
        }
        return false;
    }
}
