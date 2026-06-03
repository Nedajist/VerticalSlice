using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] AudioSource UIAudioPlayer;
    [SerializeField] AudioClip BossLevelSFX;
    [SerializeField] AudioClip WavesLevelSFX;
    [SerializeField] GameObject cursor;
    [SerializeField] Camera main_camera;
    [SerializeField] TrailRenderer cursor_trail;

    private Material cursor_material;
    private float _time = 0;
    private void Start()
    {
        Time.timeScale = 1;
        cursor_material = cursor_trail.material;

    }

    private void Update()
    {
        _time += Time.deltaTime;
        Vector3 mouse_position = main_camera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 true_position = new Vector3(mouse_position.x, mouse_position.y, 0);
        cursor.transform.position = true_position;
        cursor_material.SetVector("_color_offset", new Vector2(Mathf.Sin(_time), Mathf.Sin(_time)));
        cursor_material.SetFloat("_color_opacity", Mathf.Sin(_time));

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
