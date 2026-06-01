using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] AudioSource UIAudioPlayer;
    [SerializeField] AudioClip BossLevelSFX;
    [SerializeField] AudioClip WavesLevelSFX;

    private void Start()
    {
        Time.timeScale = 1;
    }

    public void TransitionToBossLevel()
    {
        StartCoroutine(PlaySFXThenTransitionLevels(BossLevelSFX, 1, 0.5f));

    }

    public void TransitionToSandboxLevel()
    {
        StartCoroutine(PlaySFXThenTransitionLevels(WavesLevelSFX, 2, 0.5f));
    }

    public IEnumerator PlaySFXThenTransitionLevels(AudioClip SFX, int index, float wait_duration)
    {
        UIAudioPlayer.clip = SFX;
        UIAudioPlayer.Play();
        yield return new WaitForSeconds(wait_duration);
        SceneManager.LoadScene(index);
    }

}
