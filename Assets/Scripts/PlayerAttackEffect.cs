using System.Collections;
using UnityEngine;


public class PlayerAttackEffect : MonoBehaviour
{
    public GameObject slash_effect_prefab;
    public Transform anchor_position;
    public float delay_before_damage = 2f;
    public bool isPlayer;

    private Canvas canvas;

    private void Awake()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    public void PlayAttack()
    {
        StartCoroutine(PlayAttackCoroutine());
    }

    private IEnumerator PlayAttackCoroutine()
    {
        GameObject slash = Instantiate(slash_effect_prefab, anchor_position.position,
                                        Quaternion.identity, canvas.transform);
        if (isPlayer) slash.GetComponent<SpriteRenderer>().flipX = true;
        else slash.GetComponent<SpriteRenderer>().flipX = false;
        Destroy(slash, 0.5f);

        yield return new WaitForSeconds(delay_before_damage);

        // вылетающая цифра урона

        Player.instance.damage_counter = 0;
        Player.instance.UpdateUI();
    }
}