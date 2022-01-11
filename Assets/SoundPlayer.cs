using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public AudioClip[] audioClips;
    public AudioSource audioSource;

    private void Start()
    {
        if (!Application.isEditor) Application.targetFrameRate = 300;
    }

    public void PlaySound(int sound)
    {
        try
        {
            audioSource.pitch = 1;
            audioSource.PlayOneShot(audioClips[sound]);
        }
        catch
        {
            Debug.Log("Out of Range");
        }
    }

    public void PlaySound(int sound, float pitchVariance)
    {
        try
        {
            audioSource.pitch = 1 + Random.Range(-pitchVariance, pitchVariance);
            audioSource.PlayOneShot(audioClips[sound]);
        }
        catch
        {
            Debug.Log("Out of Range");
        }
    }
}
