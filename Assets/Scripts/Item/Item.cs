using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public interface ICooldownable
{
    bool isReady();
    void TickCooldown();
    void ResetCooldown();
}

public enum ItemType
{
    Passive,
    Active
}

public enum ItemQuality
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Cursed
}

public static class ItemQualityColors
{
    public static readonly Dictionary<ItemQuality, Color> quality_colors = new Dictionary<ItemQuality, Color>
    {
        { ItemQuality.Common, Color.gray },
        { ItemQuality.Uncommon, Color.green },
        { ItemQuality.Rare, Color.cyan },
        { ItemQuality.Epic, Color.magenta },
        { ItemQuality.Legendary, Color.yellow },
        { ItemQuality.Cursed, Color.red }
    };

    public static Color GetColor(ItemQuality quality)
    {
        return quality_colors[quality];
    }
}

public abstract class Item : ScriptableObject
{
    public string name;
    public string description;
    public Sprite icon;
    public ItemType item_type;
    public ItemQuality item_quality;

    public bool isDisposable = false;
    public virtual void ApplyToPlayer(Player player) { }
    public virtual void ApplyToEnemy(Base_enemy enemy) { }
}
