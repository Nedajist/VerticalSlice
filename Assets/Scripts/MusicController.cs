using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] AudioSource _audio_player;
    [SerializeField] List<AudioClip> _tracklist; // index 0 is reserved for menu music. 
    
    public void ResetSelf()
    {
        StopAllCoroutines();
        PlayMenuMusic();
        _audio_player.volume = 1;
    }

    public void PlayMenuMusic()
    {
        _audio_player.clip = _tracklist[0];
        _audio_player.Play();
    }

    public void PlayLevelMusic()
    {
        StartCoroutine(FadeOutAndIn(2, 1)); // plays boss phase 1 music

    }

    public void PlayAtIndex(int index) // plays track at given index, slowly ramps up volume so transition is not harsh 
    {
        StartCoroutine(FadeOutAndIn(2, index));
    }

    private IEnumerator FadeOutAndIn(float duration, int index) // lowers, then raises the volume
    {
        float fade_out_timer = duration;
        float fade_in_timer = duration;

        while (fade_out_timer > 0) // audio dims 
        {
            _audio_player.volume = fade_out_timer / duration;
            fade_out_timer -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        _audio_player.clip = _tracklist[index]; // track changes 
        _audio_player.Play();

        while (fade_in_timer > 0) // audio rises 
        {
            _audio_player.volume = (1 - fade_in_timer) / duration;
            fade_in_timer -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }


    }

}
