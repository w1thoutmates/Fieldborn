using UnityEngine;

public enum ItemType
{
    Passive,
    Active
}

public abstract class Item : ScriptableObject
{
    public string name;
    public string description;
    public Sprite icon;
    public ItemType item_type;

    public bool isDisposable = false;
    public virtual void ApplyToPlayer(Player player) { }
    public virtual void ApplyToEnemy(Base_enemy enemy) { }
}
