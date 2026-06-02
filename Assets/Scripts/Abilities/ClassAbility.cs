using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClassAbility : MonoBehaviour
{
    [SerializeField] AudioSource Ability1SFXPlayer;
    [SerializeField] AudioSource Ability2SFXPlayer;

    [SerializeField] AudioClip Ability1SFX;
    [SerializeField] AudioClip Ability2SFX;

    public abstract void Ability1(Vector3 mouse_position);
    public abstract void Ability2(Vector3 mouse_position);

    protected void PlayAbility1SFX()
    {
        Ability1SFXPlayer.clip = Ability1SFX;
        Ability1SFXPlayer.Play();
    }
    protected void PlayAbility2SFX()
    {
        Ability2SFXPlayer.clip = Ability2SFX;
        Ability2SFXPlayer.Play();
    }
}
