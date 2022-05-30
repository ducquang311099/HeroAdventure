using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingController : MonoBehaviour
{
    #region ==== Fields ====
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioSource themeMusic;

    private float currentVolume;
    #endregion

    #region ==== Unity LifeCircle ====
    private void Start()
    {
        currentVolume = volumeSlider.value;
    }

    private void Update()
    {
        themeMusic.volume = volumeSlider.value;
    }
    #endregion

    #region ==== My Methods ====
    public void BackButtonClick()
    {
        gameObject.GetComponent<SlideAnimation>().CloseDialog();
        volumeSlider.value = currentVolume;
    }
    public void ApplyButtonClick()
    {
        currentVolume = volumeSlider.value;
        gameObject.GetComponent<SlideAnimation>().CloseDialog();
    }
    #endregion
}
