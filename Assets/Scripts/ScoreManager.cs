using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public AudioSource hitSFX;
    public AudioSource missSFX;
    public AudioSource failSFX;

    // attributes from other classes
    public static float volumeSFX;
    public int allNotes;
    public static int highscore;
    public static int currentStage;

    // in game UI
    public TMPro.TextMeshPro comboText;
    public TMPro.TextMeshPro scoreText;
    public TMPro.TextMeshPro accuracyText;

    public SpriteRenderer timingText;
    public Sprite[] indicatorImgs; // perfect, good, miss
    static float transparency;

    public Image healthBar; // set to mask
    public Image healthBarParent; // set to parent
    public float healthMax;
    static float healthCurrent;

    // post game UI
    public Transform failWindow;
    public Transform winWindow;
    public bool failedControl;

    public TMPro.TextMeshProUGUI comboEndText;
    public TMPro.TextMeshProUGUI scoreEndText;
    public TMPro.TextMeshProUGUI accuracyEndText;
    public TMPro.TextMeshProUGUI perfectEndText;
    public TMPro.TextMeshProUGUI goodEndText;
    public TMPro.TextMeshProUGUI missEndText;
    public TMPro.TextMeshProUGUI highscoreEndText;
    public TMPro.TextMeshProUGUI highscoreEndNotif;
    public TMPro.TextMeshProUGUI highestComboText;

    // Performance stats
    static int combo;
    static int maxCombo;
    static int score;
    static double accuracy;
    static int notesPassed;
    static int notesPerfect;
    static int notesGood;
    static int notesMissed;

    void Start()
    {
        Instance = this;
        combo = 0;
        maxCombo = 0;
        score = 0;
        accuracy = 100;
        notesPassed = 0;
        notesPerfect = 0;
        notesGood = 0;
        notesMissed = 0;
        transparency = 0;
        healthCurrent = healthMax;
        failedControl = false;
        allNotes = 0;

        Instance.hitSFX.volume = volumeSFX;
        Instance.missSFX.volume = volumeSFX;
        Instance.failSFX.volume = volumeSFX;
        failWindow.gameObject.SetActive(failedControl);
        winWindow.gameObject.SetActive(false);
        healthBarParent.gameObject.SetActive(true);

    }

    public static void Hit(int timingRating)
    {
        // update combo and max combo
        combo += 1;
        if (combo > maxCombo)
        {
            maxCombo = combo;
        }

        notesPassed += 1;
        transparency = 1;
        Instance.timingText.color = new Color(1f,244f/255f,44f/255f,transparency);

        if (timingRating == 0){ // for a perfect hit
            notesPerfect += 1;
            score += combo *30; // increase score 
        }

        else if (timingRating == 1){ // for a good hit
            notesGood += 1;
            score += combo*20; // increase score 
        }

        Instance.CalculateAcc();
        Instance.hitSFX.Play();
        Instance.timingText.sprite = Instance.indicatorImgs[timingRating];

        //checks if stage is complete
        if (notesPassed == Instance.allNotes)
        {
            Instance.StartCoroutine(Instance.StageComplete());
        }
    }

    public static void Miss()
    {
        combo = 0; // reset combo
        notesPassed += 1;
        notesMissed += 1;
        
        // take away health, unless less than 0
        healthCurrent -= 30;
        if (healthCurrent < 0)
        {
            healthCurrent = 0;
        }

        transparency = 1;
        Instance.timingText.color = new Color(1f,244f/255f,44f/255f,transparency);

        Instance.CalculateAcc(); 
        Instance.missSFX.Play();
        Instance.timingText.sprite = Instance.indicatorImgs[2];

        //checks if stage is complete
        if (notesPassed == Instance.allNotes)
        {
            Instance.StartCoroutine(Instance.StageComplete());
        }
    }

    // method for calculating accuracy proprtianlly to hit areas
    private void CalculateAcc()
    {
        accuracy = (notesPerfect + notesGood*0.66666666) / notesPassed * 100;
        accuracy = Math.Round(accuracy, 2);
    }

    private void Update()
    {
        // keybinds for return button
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnReturn();
        }

        // update HUD
        comboText.text = combo.ToString();
        scoreText.text = score.ToString();
        // add 0s to accuracy text
        if (accuracy.ToString().Length == 4)
        {
            accuracyText.text = accuracy.ToString() + "0%";
        }
        else
        {
            accuracyText.text = accuracy.ToString() + "%";
        }

        // update healthbar
        healthBar.fillAmount = healthCurrent / healthMax;

        // stage failed
        if (healthCurrent == 0 && !failedControl)
        {
            StageFailed();
        }

        FadeText();
    }

    // when stage is failed, pause all events and show fail window
    public void StageFailed()
    {
        failedControl = true;
        failWindow.gameObject.SetActive(failedControl);
        Instance.failSFX.Play();
        Conductor.Instance.StopSong();
    }

    // use IEnumerator to be able to wait
    IEnumerator StageComplete()
    {
        // wait for 3 seconds
        yield return new WaitForSecondsRealtime(3);

        // camera is rendered first, canvas second (canvas goes ontop)
        healthBarParent.gameObject.SetActive(false);

        // maxScore is the highest possible score the user can achieve
        int maxScore = 0;
        for (int i = 1; i <= Instance.allNotes; i++)
        {
            maxScore += i*30;
        }
        if (score > maxScore)  // score cannot be greater than maxScore
        {
            score = maxScore;
        }

        // update text before displaying
        comboEndText.text = "Best Combo: " + maxCombo.ToString();
        scoreEndText.text = "Score: " + score.ToString();
        highscoreEndText.text = "High Score: " + highscore.ToString();
        accuracyEndText.text = "Accuracy: " + accuracy.ToString() + "%";
        perfectEndText.text = "Perfects: " + notesPerfect.ToString();
        goodEndText.text = "Goods: " + notesGood.ToString();
        missEndText.text = "Misses: " + notesMissed.ToString();
        highestComboText.text = "Highest Possible Combo: " + allNotes.ToString();
        highscoreEndNotif.text = " "; // empty text

        // set new highscore
        if (score > highscore)
        {
            highscore = score;
            // saves highscore from stageManager class
            StageManager.setHighscore(currentStage, highscore, maxCombo);
            // show message to user
            highscoreEndNotif.text = "New High Score!";
        }

        winWindow.gameObject.SetActive(true);
    }

    // fades out timing indicator text with exponential interpolation
    public void FadeText()
    {
        if (transparency > 0)
        {
            // animation takes 1/constant after delta time, in this case 1/2 seconds
            transparency = Mathf.Pow((transparency*transparency*transparency) - Time.deltaTime*2, 1f/3f);
            timingText.color = new Color(1f,244f/255f,44f/255f,transparency); // changes alpha value
        }
        else{
            transparency = 0;
        }
    }

    public void OnReturn()
    {
        SceneManager.LoadScene("StageSelect");
    }
}

