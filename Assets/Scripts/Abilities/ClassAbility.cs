using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClassAbility : MonoBehaviour
{
    [SerializeField] AudioSource AbilitySFXPlayer;
    [SerializeField] AudioClip Ability1SFX;
    [SerializeField] AudioClip Ability2SFX;

    public abstract void Ability1(Vector3 mouse_position);
    public abstract void Ability2(Vector3 mouse_position);

    protected void PlayAbiltiy1SFX()
    {
        AbilitySFXPlayer.clip = Ability1SFX;
        AbilitySFXPlayer.Play();
    }
    protected void PlayAbility2SFX()
    {
        AbilitySFXPlayer.clip = Ability2SFX;
        AbilitySFXPlayer.Play();
    }
}
