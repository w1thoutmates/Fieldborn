using System.Collections;
using UnityEngine;


public class PlayerAttackEffect : MonoBehaviour
{
    public GameObject damage_popup_prefab;
    public Transform popup_anchor;
    public GameObject slash_effect_prefab;
    public Transform anchor_position;
    public float delay_before_damage = 0f;
    public bool isPlayer;

    private Canvas canvas;

    private void Awake()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    public IEnumerator PlayAttack(int damage, bool isCrit)
    {
        GameObject slash = Instantiate(slash_effect_prefab, anchor_position.position,
                                        Quaternion.identity, canvas.transform);
        if (isPlayer) slash.GetComponent<SpriteRenderer>().flipX = true;
        else slash.GetComponent<SpriteRenderer>().flipX = false;
        Destroy(slash, 0.5f);

        CameraShaker.instance.Shake(0.3f, isCrit ? 0.4f : 0.2f);

        GameObject damage_popup = Instantiate(damage_popup_prefab, popup_anchor.position, Quaternion.identity, canvas.transform);
        damage_popup.GetComponent<DamagePopup>().Setup(damage, isCrit ? new Color(255f / 255f, 172f / 255f, 0f / 255f, 1f) : Color.red, isCrit);

        yield return new WaitForSeconds(0.1f);
    }
}