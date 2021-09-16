using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour
{
    public static void SetUpAudioSourcePrefab(AudioClip audioClip)
    {
        GameObject audio = Instantiate(Resources.Load<GameObject>("Prefabs/AudioSource"));
        AudioSource audioSource = audio.GetComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.Play();
        Destroy(audio, audioClip.length);
    }

    public static void CreateParticleSystem(ParticleSystem particleSystem, Vector3 pos)
    {
        ParticleSystem ps = Instantiate(particleSystem, pos, Quaternion.identity);
        ps.Play();
    }
}