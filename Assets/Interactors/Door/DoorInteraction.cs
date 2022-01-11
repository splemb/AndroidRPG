using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : Interactor
{
    [SerializeField] Transform pivot;
    bool open = false;

    [SerializeField] AudioClip[] sounds;
    [SerializeField] AudioSource audioSource;

    public override void Interact()
    {
        open = !open;

        if (open)
        {
            pivot.localEulerAngles = Vector3.up * -90f;
            audioSource.PlayOneShot(sounds[0]);
        }
        else
        {
            pivot.localEulerAngles = Vector3.zero;
            audioSource.PlayOneShot(sounds[1]);
        }
    }
}
