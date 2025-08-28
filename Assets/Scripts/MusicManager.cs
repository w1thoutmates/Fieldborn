using System;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    private AudioSource audio_source;

    public float max_vol = 1.0f;
    public float fade_time = 2f;
    public float pause_btw_loop = 2f;

    private void Awake()
    {
        audio_source = GetComponent<AudioSource>();
        audio_source.volume = 0f;
        audio_source.loop = false;
        StartCoroutine(PlayLoop());
    }
    
    private System.Collections.IEnumerator PlayLoop()
    {
        while(true)
        {
            audio_source.Play();

            float t = 0;
            while (t < fade_time)
            {
                t += Time.deltaTime;
                audio_source.volume = Mathf.Lerp(0, max_vol, t / fade_time);
                yield return null;
            }
            audio_source.volume = max_vol;

            yield return new WaitForSeconds(audio_source.clip.length - fade_time);

            float start_volume = audio_source.volume;
            t = 0;
            while (t < fade_time)
            {
                t += Time.deltaTime;
                audio_source.volume = Mathf.Lerp(start_volume, 0, t / fade_time);
                yield return null;
            }
            audio_source.volume = 0;
            audio_source.Stop();

            yield return new WaitForSeconds(pause_btw_loop);
        }
    }
}
