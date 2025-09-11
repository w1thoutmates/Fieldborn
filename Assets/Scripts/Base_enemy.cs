using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public interface IHealable
{
    IEnumerator HealSelf();
}

public interface IShieldable
{
    IEnumerator CastShield();
}


public abstract class Base_enemy : MonoBehaviour
{
    [Header("counters")]
    public int damage_counter; // red counter
    public TextMeshProUGUI damage_counter_text;

    [Header("health")]
    public float max_health;
    public float current_health;
    public GameObject health_bar_prefab;
    public Transform health_bar_anchor;
    protected TextMeshProUGUI health_text;
    [HideInInspector]public health_bar health_bar_instance;

    [Header("leveling")]
    public float max_exp;
    public float current_exp = 1;
    public int max_level;
    public int current_level = 1;

    protected float crit_chance = 0.10f;
    protected int crit_multiplier = 2;

    [Header("sounds")]
    public AudioClip taking_damage_sound;
    [HideInInspector] public AudioSource audio_source;

    [Header("other")]
    public Transform hand_anchor_pos;
    [HideInInspector] public Inventory inventory;
    private GameObject shapes_on_scene;

    protected virtual void Awake() 
    {
        inventory = GetComponent<Inventory>();
        audio_source = GetComponent<AudioSource>();
        shapes_on_scene = GameObject.Find("Shapes");
    }

    protected virtual void Start()
    {
        current_health = max_health;
        current_level = Player.instance.current_level;

        GameObject hb = Instantiate(health_bar_prefab, health_bar_anchor);
        hb.transform.localPosition = Vector3.zero;

        health_bar_instance = hb.GetComponent<health_bar>();
        health_bar_instance.SetMaxHealth(max_health);
        health_bar_instance.UpdateHealthBar(current_health);
        health_text = hb.GetComponentInChildren<TextMeshProUGUI>();
        UpdateHealthBarText();
    }

    public virtual void GainExp(float exp)
    {
        current_exp += exp;
        GainLevel();
    }

    public virtual void GainLevel()
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

    public virtual void SetHealth(float health)
    {
        current_health = health;
    }

    public virtual bool RollCrit()
    {
        if (Random.value < crit_chance)
        {
            return true;
        }
        return false;
    }

    public virtual void UpdateUI()
    {
        damage_counter_text.text = damage_counter.ToString();
    }

    public virtual int TakeDamage(float damage)
    {
        current_health -= damage;
        UpdateUI();

        if (health_bar_instance != null)
        {
            health_bar_instance.UpdateHealthBar(current_health);
            UpdateHealthBarText();
        }

        audio_source.PlayOneShot(taking_damage_sound);

        if (current_health <= 0)
        {
            current_health = 0;
            Die();
        }

        EventBus.Instance.enemyTakenDamage?.Invoke((int)damage);

        return (int)damage;
    }

    public virtual void TakePureDamage(float damage)
    {
        current_health -= damage;
        UpdateUI();
        health_bar_instance?.UpdateHealthBar(current_health);
        UpdateHealthBarText();

        audio_source.PlayOneShot(taking_damage_sound);

        if (current_health <= 0)
        {
            current_health = 0;
            Die();
        }

        Debug.Log($"<color=red>ВРАГ ПОЛУЧИЛ {damage} чистого урона от отражения</color>");
    }

    public virtual void TurnActions()
    {
        Debug.Log("Враг походил");
        StartCoroutine(EnemyTurnDelay());
    }

    public IEnumerator EnemyTurnDelay()
    {
        if (PauseManager.isPaused)
            yield return new WaitWhile(() => PauseManager.isPaused);

        shapes_on_scene.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        shapes_on_scene.gameObject.SetActive(true);
        TurnManager.instance.EndTurn();
    }

    public virtual IEnumerator Attack()
    {
        if (PauseManager.isPaused)
            yield return new WaitWhile(() => PauseManager.isPaused);

        if (damage_counter <= 0) yield break;

        TurnManager.instance.turn_text.text = $"<size=80%>Враг атакует</size>";
        TurnManager.instance.turn_text.color = new Color(255f / 255f, 91f / 255f, 76f / 255f, 1f);

        // обработка случая, в котором во время атаки цель может погибнуть

        PlayerAttackEffect effect = GetComponent<PlayerAttackEffect>();

        int damage = damage_counter;
        bool crit = RollCrit();

        if (crit) damage *= crit_multiplier;

        if (effect != null) 
            yield return StartCoroutine(effect.PlayAttack(damage, crit));

        int final_damage = Player.instance.TakeDamage(damage);

        UpdateUI();

        // other actions (soon)

        yield return new WaitForSeconds(1f);
    }

    public virtual void Die()
    {
        Destroy(gameObject);
        Destroy(health_bar_instance.gameObject);
        health_bar_instance = null;
        BattleOutcomeManager.instance.ShowWinScreen();
    }

    public virtual void UpdateHealthBarText()
    {
        health_text.text = current_health.ToString() + "/" + max_health.ToString();
    }

      // В будущем добавить логику предметов:
     // foreach(var item in items) item.ApplyEffect(); (с задержкой)
}

