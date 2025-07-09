using UnityEngine;

public class Ork_enemy : Base_enemy
{
    protected override void Awake()
    {
        if (damage_counter_text == null)
        {
            Debug.LogWarning("damage_text isn't identity for enemy");
            return;
        }
    }

    protected override void Start()
    {
        base.Start();
        damage_counter = Player.instance.current_level + 1;
        UpdateUI();
    }
}
