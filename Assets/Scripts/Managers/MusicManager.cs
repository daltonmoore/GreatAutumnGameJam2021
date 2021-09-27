using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    static MusicManager instance;
    public static MusicManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MusicManager>();
            }
            return instance;
        }
    }

    public List<AudioClip> tracks;
    public AudioMixer mixer;
    public static string PlayerPrefsMasterVolume = "PPMasterVolume";

    AudioSource mainSceneAudioSource;
    Coroutine trackDelay;
    bool inMainScene = false;
    string MasterAudioGroupVolume = "MasterVolume";
    string MainScene = "MainScene";

    public void ChangeMusicVolume(float val)
    {
        float volume = Util.LinearToDB(val);
        mixer.SetFloat(MasterAudioGroupVolume, volume);
        PlayerPrefs.SetFloat(PlayerPrefsMasterVolume, val);
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        mainSceneAudioSource = GetComponent<AudioSource>();
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

        if (PlayerPrefs.HasKey(PlayerPrefsMasterVolume))
        {
            mixer.SetFloat(MasterAudioGroupVolume, Util.LinearToDB(PlayerPrefs.GetFloat(PlayerPrefsMasterVolume)));
        }
    }

    private void SceneManager_activeSceneChanged(Scene leftScene, Scene newScene)
    {
        if (leftScene.name == MainScene)
        {
            inMainScene = false;
            mainSceneAudioSource.Stop();
        }
        else if (newScene.name == MainScene)
        {
            inMainScene = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //#if UNITY_EDITOR
        //if (Input.GetKeyDown("f"))
        //{
        //    PlayerPrefs.DeleteAll();
        //    Debug.Log("Deleted player prefs");
        //}
        //#endif

        if (inMainScene)
            PlayRandomSong();
    }

    void PlayRandomSong()
    {
        if (trackDelay == null)
        {
            int randomIndex = Random.Range(0, tracks.Count - 1);
            mainSceneAudioSource.clip = tracks[randomIndex];
            mainSceneAudioSource.Play();
            trackDelay = StartCoroutine(TrackDelay(mainSceneAudioSource.clip.length));
        }
    }

    IEnumerator TrackDelay(float d)
    {
        yield return new WaitForSeconds(d);
        trackDelay = null;
    }
}
