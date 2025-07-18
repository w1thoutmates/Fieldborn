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
}
