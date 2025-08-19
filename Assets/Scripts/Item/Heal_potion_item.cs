using UnityEngine;

[CreateAssetMenu(menuName = "Items/Active/Heal_potion")]
public class Heal_potion_item : Item
{
    public int heal_value = 5;

    public override void ApplyToPlayer(Player pl)
    {
        if (!TurnManager.instance.IsPlayerTurn()) return;

        pl.current_health = Mathf.Min(pl.max_health, pl.current_health + heal_value);
        pl.health_bar_instance.UpdateHealthBar(pl.current_health);
        pl.UpdateHealthBarText();

        GameObject heal_popup = Instantiate(pl.popup_prefab, pl.popup_anchor.position, Quaternion.identity, FindObjectOfType<Canvas>().transform);
        heal_popup.GetComponent<DamagePopup>().Setup(heal_value, new Color(129f / 255f, 199f / 255f, 132f / 255f));
    }

    public override void ApplyToEnemy(Base_enemy en)
    {
        if (!TurnManager.instance.IsEnemyTurn()) return;

        en.current_health = Mathf.Min(en.max_health, en.current_health + heal_value);
        en.health_bar_instance.UpdateHealthBar(en.current_health);
        en.UpdateHealthBarText();
    }
}
