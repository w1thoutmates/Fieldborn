using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using Sequence = DG.Tweening.Sequence;

[CreateAssetMenu(menuName = "Items/Passive/Magic_scepter")]
public class Magic_scepter_item : Item, ICooldownable
{
    public AudioClip magic_missile_sound;
    public GameObject magic_missile_prefab;
    private int cooldown = 2;
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

    public override void ApplyToEnemy(Base_enemy enemy)
    {
        if(!isReady()) return;

        enemy.StartCoroutine(ItemLogic(enemy));
        ResetCooldown();
    }
    
    private IEnumerator ItemLogic(Base_enemy enemy)
    {
        var cell = Grid.instance.GetRandomOccupiedCell();

        if (cell == null) yield break;

        // start animation

        var cell_start_transform = cell.transform.position;

        Tweener shake_tween = cell.transform.DOShakePosition(
            duration: 1.3f,   
            strength: 3f,
            vibrato: 50,
            randomness: 90,
            fadeOut: false   
        ).SetLoops(-1, LoopType.Restart);

        yield return UIZoomManager.instance.StartCoroutine(UIZoomManager.instance.Zoom(1.02f, 1.35f));

        GameObject missile = Instantiate(
            magic_missile_prefab,
            enemy.hand_anchor_pos.position,
            Quaternion.identity,
            Grid.instance.transform.parent 
        );

        enemy.audio_source.PlayOneShot(magic_missile_sound);

        yield return MoveMissile(missile, cell.transform.position);

        var particles = Grid.instance.destroy_square_prefab;

        if (enemy is Wizard_enemy wizard)
        {
            switch (cell.cellType)
            {
                case CellType.Damage:
                    wizard.damage_counter += Player.instance.current_level;
                    break;
                case CellType.Heal:
                    wizard.heal_counter += Player.instance.current_level;
                    break;
                case CellType.Shield:
                    wizard.shield_counter += Player.instance.current_level;
                    break;
            }
            wizard.UpdateUI();
        }

        cell.Deactivate();
        cell.ClearOccupied();
        GameObject.Instantiate(particles, cell.transform.position, Quaternion.identity).GetComponent<SquareExplosionEffect>().Play(cell.activeImage.color);

        shake_tween.Kill();
        cell.transform.position = cell_start_transform;

        yield return null;
    }

    private IEnumerator MoveMissile(GameObject missile, Vector3 target_pos)
    {
        /*
    float duration = 0.2f; 
    float elapsed = 0f;

    Vector3 start_pos = missile.transform.position;

    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = elapsed / duration;

        missile.transform.position = Vector3.Lerp(start_pos, target_pos, t);
        
        yield return null;
    }
    missile.transform.position = target_pos;
    Destroy(missile);

        */
        float duration = 0.3f;
        float arcHeight = 1.5f;

        Vector3 midPoint = (missile.transform.position + target_pos) / 2 + Vector3.up;
        Vector3[] path = new Vector3[] { missile.transform.position, midPoint, target_pos };

        missile.transform.DOPath(
            path: path,
            duration: duration,
            pathType: PathType.CatmullRom
        ).SetEase(Ease.InOutSine)
         .OnComplete(() => Destroy(missile));

        yield return new WaitForSeconds(duration);
    }
}

