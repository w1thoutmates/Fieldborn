using System.Collections;
using UnityEngine;
using UnityEngine.Audio;


public class PlayerAttackEffect : MonoBehaviour
{
    public GameObject damage_popup_prefab;
    public Transform popup_anchor;
    public GameObject slash_effect_prefab;
    public Transform anchor_position;
    public float delay_before_damage = 0f;
    public bool isPlayer;
    public AudioClip[] slashes;

    private Canvas canvas;
    private AudioSource audio_source;

    private void Awake()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        audio_source = this.GetComponent<AudioSource>();
    }

    public IEnumerator PlayAttack(int damage, bool isCrit)
    {
        if (PauseManager.isPaused)
        {
            yield return new WaitWhile(() => PauseManager.isPaused);
        }

        if (slashes != null && slashes.Length > 0)
        {
            int random_index = Random.Range(0, slashes.Length);
            audio_source.PlayOneShot(slashes[random_index]);
        }

        GameObject slash = Instantiate(slash_effect_prefab, anchor_position.position,
                                        Quaternion.identity, canvas.transform);
        if (isPlayer) slash.GetComponent<SpriteRenderer>().flipX = true;
        else slash.GetComponent<SpriteRenderer>().flipX = false;
        Destroy(slash, 0.5f);

        FindObjectOfType<CameraShaker>().Shake(isCrit ? 0.35f : 0.25f, isCrit ? 10f : 7f);

        GameObject damage_popup = Instantiate(damage_popup_prefab, popup_anchor.position, Quaternion.identity, canvas.transform);
        damage_popup.GetComponent<DamagePopup>().Setup(damage, isCrit ? new Color(229f / 255f, 115f / 255f, 115f / 255f) : new Color(255f / 255f, 172f / 255f, 0f / 255f, 1f), isCrit);

        yield return new WaitForSeconds(0.1f);
    }
}