using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public List<AudioClip> tracks;

    AudioSource audioSource;
    Coroutine trackDelay;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayRandomSong();
    }

    void PlayRandomSong()
    {
        if (trackDelay == null)
        {
            int randomIndex = Random.Range(0, tracks.Count - 1);
            audioSource.clip = tracks[randomIndex];
            audioSource.Play();
            trackDelay = StartCoroutine(TrackDelay(audioSource.clip.length));
        }
    }

    IEnumerator TrackDelay(float d)
    {
        yield return new WaitForSeconds(d);
        trackDelay = null;
    }
}
