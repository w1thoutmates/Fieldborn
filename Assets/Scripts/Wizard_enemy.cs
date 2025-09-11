using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Wizard_enemy : Base_enemy, IHealable, IShieldable
{
    [Header("counters")]
    public int heal_counter; // green counter
    public TextMeshProUGUI heal_counter_text;

    public int shield_counter; // blue counter
    public TextMeshProUGUI shield_counter_text;

    private int active_shield;

    [Header("visuals")]
    public Transform popup_anchor;
    public GameObject popup_prefab;

    [Header("sounds")]
    public AudioClip healself_sound;
    public AudioClip shield_sound;

    private Canvas canvas;


    protected override void Awake()
    {
        base.Awake();
        if (damage_counter_text == null)
        {
            Debug.LogWarning("damage_text isn't identity for enemy");
            return;
        }
        if(shield_counter_text == null)
        {
            Debug.LogWarning("heal_text isn't identity for enemy");
            return;
        }
        if(heal_counter_text == null)
        {
            Debug.LogWarning("shield_text isn't identity for enemy");
            return;
        }

        canvas = FindObjectOfType<Canvas>();
    }

    protected override void Start()
    {
        base.Start();
        damage_counter = Player.instance.current_level;
        UpdateUI();
    }

    public override void TurnActions()
    {
        foreach(var item in inventory.items)
        {
            if(item is ICooldownable cooldown)
            {
                if (cooldown.isReady())
                {
                    item.ApplyToEnemy(this);
                }
                cooldown.TickCooldown();
            }
        }

        base.TurnActions();
    }

    public override int TakeDamage(float damage)
    {
        int int_damage = Mathf.RoundToInt(damage);

        int shield_reduction = Mathf.Min(active_shield, int_damage);
        active_shield -= shield_reduction;
        Debug.Log($"<color=blue>ÂÐÀÃ ÏÎÃËÎÒÈË {shield_reduction} ÓÐÎÍÀ ÙÈÒÎÌ</color>");
        int final_damage = int_damage - shield_reduction;

        current_health -= final_damage;

        UpdateUI();
        health_bar_instance?.UpdateHealthBar(current_health);
        UpdateHealthBarText();
        audio_source.PlayOneShot(taking_damage_sound);

        if (current_health <= 0) Die();

        EventBus.Instance.enemyTakenDamage?.Invoke(int_damage);

        return final_damage;
    }

    public override IEnumerator Attack()
    {
        yield return base.Attack();
        damage_counter = Player.instance.current_level;
        UpdateUI();
    }

    public IEnumerator HealSelf()
    {
        if (PauseManager.isPaused)
            yield return new WaitWhile(() => PauseManager.isPaused);

        if (heal_counter <= 0) yield break;

        TurnManager.instance.turn_text.text = $"<size=80%>Èãðîê ëå÷èòñÿ</size>";
        TurnManager.instance.turn_text.color = new Color(149f / 255f, 255f / 255f, 140f / 255f, 1f);

        audio_source.PlayOneShot(healself_sound);
        current_health = Mathf.Min(max_health, current_health + heal_counter);
        health_bar_instance.UpdateHealthBar(current_health);
        UpdateHealthBarText();

        GameObject heal_popup = Instantiate(popup_prefab, popup_anchor.position, Quaternion.identity, canvas.transform);
        heal_popup.GetComponent<DamagePopup>().Setup(heal_counter, new Color(129f / 255f, 199f / 255f, 132f / 255f));

        heal_counter = 0;
        UpdateUI();
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator CastShield()
    {
        if (PauseManager.isPaused)
            yield return new WaitWhile(() => PauseManager.isPaused);

        if (shield_counter <= 0) yield break;

        TurnManager.instance.turn_text.text = $"<size=80%>Ïðèìåíÿåòñÿ ùèò</size>";
        TurnManager.instance.turn_text.color = new Color(149f / 255f, 255f / 255f, 140f / 255f, 1f);

        audio_source.PlayOneShot(shield_sound);
        GameObject shield_popup = Instantiate(popup_prefab, popup_anchor.position, Quaternion.identity, canvas.transform);
        shield_popup.GetComponent<DamagePopup>().Setup(shield_counter, new Color(100f / 255f, 181f / 255f, 246f / 255f));

        // animation

        /* yield return new WaitForSeconds(0.5f); delay for animation */

        active_shield = shield_counter;
        shield_counter = 0;
        UpdateUI();

        yield return new WaitForSeconds(1f);
    }

    public override void UpdateUI()
    {
        base.UpdateUI(); 
        heal_counter_text.text = heal_counter.ToString();
        shield_counter_text.text = shield_counter.ToString();
    }

}
