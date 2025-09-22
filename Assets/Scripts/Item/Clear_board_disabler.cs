using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Items/Active/Clear_board_disabler")]

public class Clear_board_disabler : Item, ICooldownable
{
    public bool disable_clear_board = true;
    private int cooldown = 4;
    private int cooldown_timer = 0;

    public void TickCooldown()
    {
        if (cooldown_timer > 0) cooldown_timer--;
    }

    public bool isReady()
    {
        return cooldown_timer <= 0;
    }

    public void ResetCooldown()
    {
        cooldown_timer = cooldown;
    }

    public override void ApplyToPlayer(Player player)
    {
        if (!isReady()) return;

        player.StartCoroutine(ItemLogic());
        ResetCooldown();
    }

    private IEnumerator ItemLogic()
    {
        if (disable_clear_board)
        {
            yield return Grid.instance.ClearBoardWithDelay();
        }
    }
}
