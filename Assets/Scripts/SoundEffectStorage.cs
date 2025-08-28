using UnityEngine;

public class SoundEffectStorage : MonoBehaviour
{
    public static SoundEffectStorage instance;

    [HideInInspector] public AudioSource audio_source;
    public AudioClip pop_sound;
    public AudioClip success_sound;
    public AudioClip deny_sound;
    

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        audio_source = GetComponent<AudioSource>();   
    }

    public void ButtonSound(AudioClip ac)
    {
        audio_source.PlayOneShot(ac);
    }
}
