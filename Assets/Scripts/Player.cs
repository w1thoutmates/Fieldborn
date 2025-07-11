using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    public static Player instance;

    [Header("counters")]
    public int damage_counter; // red counter
    public TextMeshProUGUI damage_counter_text;
    public int heal_counter; // green counter
    public TextMeshProUGUI heal_counter_text;
    public int shield_counter; // blue counter
    public TextMeshProUGUI shield_counter_text;
    public GameObject choice_menu;

    [Header("health")]
    public float max_health;
    public float current_health;
    public GameObject health_bar_prefab;
    public Transform health_bar_anchor;
    private TextMeshProUGUI health_text;
    [HideInInspector]public health_bar health_bar_instance;

    [Header("leveling")]
    public float max_exp;
    public float current_exp = 1;
    public int max_level;
    public int current_level = 1;

    private float crit_chance = 0.15f;
    private int crit_multiplier = 2;

    private int placedShapesThisTurn = 0;
    private int maxShapesPerTurn = 3;

    private int active_shield = 0;

    // Добавить ОСОБЫЙ эффект если ВСЯ ЗАВЕРШЕННАЯ ЛИНИЯ ПОЛНОСТЬЮ будет только ИЗ ОДНОГО ЦВЕТА
    // Механику уклонения добавить в игру
    // Добавить - если нет предметов для использования и все фигуры поставлены - ход автоматически заканчивается
    // или включать эту функцию по кнопке

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (damage_counter_text == null || heal_counter_text == null || shield_counter_text == null)
        {
            Debug.LogWarning("damage_text or heal_text or shield_text isn't identity");
            return;
        }
    }

    private void Start()
    {
        current_health = max_health;

        var canvasTransform = GameObject.Find("Canvas").transform;
        GameObject hb = Instantiate(health_bar_prefab, health_bar_anchor.position, Quaternion.identity, canvasTransform);
        health_bar_instance = hb.GetComponent<health_bar>();
        health_bar_instance.SetMaxHealth(max_health);
        health_bar_instance.UpdateHealthBar(current_health);
        health_text = hb.GetComponentInChildren<TextMeshProUGUI>();
        UpdateHealthBarText();
    }

    public void GainExp(float exp)
    {
        current_exp += exp;
        GainLevel();
    }

    public void GainLevel()
    {
        if (current_exp >= max_exp)
        {
            current_level++;
            float overflow = current_exp - max_exp;
            current_exp = overflow;
            max_exp *= 1.25f;
            max_health += 5;
        }
    }

    public void SetHealth(float health)
    {
        current_health = health;
    }

    public bool RollCrit()
    {
        if (Random.value < crit_chance)
        {
            // damage_counter *= crit_multiplier;
            return true;
        }
        return false;
    }

    public int TakeDamage(float damage)
    {
        int int_damage = Mathf.RoundToInt(damage);

        int shield_reduction = Mathf.Min(active_shield, int_damage);
        active_shield -= shield_reduction;
        Debug.Log($"<color=blue>ИГРОК ПОГЛОТИЛ {shield_reduction} УРОНА ЩИТОМ</color>");

        int final_damage = int_damage - shield_reduction;
        current_health -= final_damage;

        UpdateUI();
        health_bar_instance?.UpdateHealthBar(current_health);
        UpdateHealthBarText();

        if (current_health <= 0) Die();
        
        return final_damage;
    }

    public void UpdateHealthBarText()
    {
        health_text.text = current_health.ToString() + "/" + max_health.ToString();
    }

    public void UpdateUI()
    {
        damage_counter_text.text = damage_counter.ToString();
        heal_counter_text.text = heal_counter.ToString();
        shield_counter_text.text = shield_counter.ToString();
    }

    public void RequestNewShapesForPlayer()
    {
        GameEvents.RequestNewShapes();
        TakeDamage(2);
    }

    public bool CanPlaceShape()
    {
        return placedShapesThisTurn < maxShapesPerTurn;
    }

    public void OnShapePlaced()
    {
        placedShapesThisTurn++;
        if(placedShapesThisTurn >= maxShapesPerTurn)
        {
            Debug.Log("На этот ход фигуры кончились");
        }
    }

    public void StartTurn()
    {
        placedShapesThisTurn = 0;
    }

    public void TurnActions()
    {
        Debug.Log($"Игрок действует");
        // активировать кнопку завершения хода
    }

    public IEnumerator Attack(Base_enemy target)
    {
        if (PauseManager.isPaused)
            yield return new WaitWhile(() => PauseManager.isPaused);

        if (damage_counter <= 0) yield break;

        PlayerAttackEffect effect = GetComponent<PlayerAttackEffect>();

        if (target != null && damage_counter > 0)
        {
            int damage = damage_counter;
            bool crit = RollCrit();

            if (crit) damage *= crit_multiplier;
            if (effect != null) yield return StartCoroutine(effect.PlayAttack(damage, crit));

            Debug.Log($"<color=red>ИГРОК УДАРИЛ НА {damage}</color>");
            target.TakeDamage(damage);

            damage_counter = 0;
            UpdateUI();
            yield return new WaitForSeconds(1f);
        }     
    }

    public IEnumerator HealSelf()
    {
        if (PauseManager.isPaused)
            yield return new WaitWhile(() => PauseManager.isPaused);

        if (heal_counter <= 0) yield break;

        current_health = Mathf.Min(max_health, current_health + heal_counter);
        health_bar_instance.UpdateHealthBar(current_health);
        UpdateHealthBarText();
        Debug.Log($"<color=green>ИГРОК ИЗЛЕЧИЛСЯ НА {heal_counter}</color>");

        heal_counter = 0;
        UpdateUI();
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator CastShield()
    {
        if (PauseManager.isPaused)
            yield return new WaitWhile(() => PauseManager.isPaused);

        if (shield_counter <= 0) yield break;

        // animation

        /* yield return new WaitForSeconds(0.5f); delay for animation */

        active_shield = shield_counter;
        shield_counter = 0;
        UpdateUI();

        Debug.Log($"<color=blue>ИГРОК КАСТУЕТ ЩИТ ({active_shield})</color>");
        yield return new WaitForSeconds(1f);
    }

    public void Die()
    {
        Destroy(gameObject);
        Destroy(health_bar_instance.gameObject);
        health_bar_instance = null;
        // GAME OVER Screen what ever.
    }
}
