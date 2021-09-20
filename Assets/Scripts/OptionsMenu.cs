using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider MusicSlider;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey(MusicManager.PlayerPrefsMasterVolume))
            MusicSlider.value = PlayerPrefs.GetFloat(MusicManager.PlayerPrefsMasterVolume);
    }

    public void ChangeMusicVolume(float val)
    {
        MusicManager.Instance.ChangeMusicVolume(val);
    }
}
