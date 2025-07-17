using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Passive/Thorns")]
public class Thorns_item : Item
{
    public int reflect_percent = 75;

    public override void ApplyToPlayer(Player player)
    {
        EventBus.Instance.playerTakenDamage += OnPlayerTakeDamage;
    }

    public override void ApplyToEnemy(Base_enemy enemy)
    {
        EventBus.Instance.enemyTakenDamage += OnEnemyTakeDamage;
    }

    private void OnPlayerTakeDamage(int damage)
    {
        int reflected = damage * reflect_percent / 100;
        Base_enemy enemy = FindObjectOfType<Base_enemy>();
        enemy.TakePureDamage(reflected);
    }

    private void OnEnemyTakeDamage(int damage)
    {
        int reflected = damage * reflect_percent / 100;
        Player.instance.TakePureDamage(reflected);
    }

}
