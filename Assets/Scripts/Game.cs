using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Game : MonoBehaviour
{
    public Slider musicSlider; // between 0 and 1
    public Slider sfxSlider; // between 0 and 1
    public Slider speedSlider; // between 0.5 and 2
    public float speed;

    public TMPro.TextMeshProUGUI musicVolText;
    public TMPro.TextMeshProUGUI SFXVolText;
    public TMPro.TextMeshProUGUI NoteSpeedText;

    public ParticleSystem particleSystem1;

    public Image gameTitle;
    public Sprite[] rhythm;
    public static int seededTitle = -1;

    void Start()
    {
        RandomInt(13);

        Application.targetFrameRate = 300;

        // set menu title on launch
        if (seededTitle == -1)
        {
            seededTitle = RandomInt(rhythm.Length);
        }
        gameTitle.sprite = rhythm[seededTitle];

        // start partice loop partway through
        particleSystem1.Simulate(UnityEngine.Random.Range(0f, 30f));
        particleSystem1.Play();

        // loads user settings
        // on slider changed method is automatically called and settings are validated.
        Load();
    }

    // linear congruential rng generation
    private int RandomInt(int upper)
    {
        // starting seed
        int x = System.DateTime.Now.Second;
        int k = 11;
        int c = 36;

        for (int i=0; i<System.DateTime.Now.Minute; i++)
        {
            // using mod 100 because no 12 long repeating sequences exist afaik
            x = (k * x + c) % 100;
        }
        return (x % upper);
    }

    // called once slider is moved
    public void OnSliderChanged()
    {
        // validate music volume slider
        if (musicSlider.value < 0 || musicSlider.value > 1)
        {
            // Invalid music volume, reset to default value
            musicSlider.value = 0.5f;
        }
        //round to nearest whole number, convert to percentage and display
        musicVolText.text = (Math.Round(musicSlider.value* 100, 0)).ToString() + "%";

        // validate sfx volume slider
        if (sfxSlider.value < 0 || sfxSlider.value > 1)
        {
            // Invalid sound effects volume, reset to default value
            sfxSlider.value = 0.5f;
        }
        //round to nearest whole number, convert to percentage and display
        SFXVolText.text = (Math.Round(sfxSlider.value * 100, 0)).ToString() + "%";

        // convert slider value to float between 0.5 and 2
        speed = 2.5f - speedSlider.value;

        // validate new speed
        if (speed < 0.5 || speed > 2)
        {
            // Invalid note speed, speed needs to be reset and not slider.
            speed = 1f;
        }
        // display slider value and not actua value for human readability.
        NoteSpeedText.text = (Math.Round(speedSlider.value, 2)).ToString();
    }

    public void OnStartClicked()
    {
        // save user settings
        Save();

        // pass values to conductor and score manager classes
        Conductor.noteTime = speed;
        Conductor.volumeMusic = musicSlider.value;
        ScoreManager.volumeSFX = sfxSlider.value;

        SceneManager.LoadScene("StageSelect");
    }

    public void OnExitClicked()
    {
        // save user settings before quiting
        Save();
        Application.Quit();
    }

    public void Save()
    {
        //passes a referance to the game class
        SaveSystem.SaveGameData(this);
    }

    public void Load()
    {
        // gets game data from file
        GameData data = SaveSystem.LoadGameData();

        if (data != null)
        {
            musicSlider.value = data.musicVolume;
            sfxSlider.value = data.sfxVolume;
            speedSlider.value = data.noteSpeed;
        }
        else
        {
            Debug.Log("No save made");
            Save();
        }
    }
}   
