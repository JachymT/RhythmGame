using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    public TMPro.TextMeshProUGUI songText;
    public TMPro.TextMeshProUGUI artistText;
    public TMPro.TextMeshProUGUI difficultyText;
    public TMPro.TextMeshProUGUI lengthText;
    public TMPro.TextMeshProUGUI bpmText;
    public TMPro.TextMeshProUGUI highScoreText;
    public TMPro.TextMeshProUGUI highComboText;
    public Image iconImage;

    public ParticleSystem particleSystem2;
    public AudioSource buttonSound;

    public Stage[] stages; // array of game objects
    public List<int> highScores = new List<int>(); // list of highscores
    public List<int> highCombos = new List<int>(); // list of highcombos
    
    void Start()
    {
        Instance = this;
        UpdateSidePanel(0); // first stage is automatically selected

        // start partice loop partway through
        particleSystem2.Simulate(UnityEngine.Random.Range(0f, 30f));
        particleSystem2.Play();

        buttonSound.volume = ScoreManager.volumeSFX;

        // use co-routine to wait for stages to fully load
        Instance.StartCoroutine(Instance.ReadHighscores());
    }

    void Update()
    {
        // keybinds for return button
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnReturnClicked();
        }
    }

    IEnumerator ReadHighscores()
    {
        // wait for 0.1 seconds
        yield return new WaitForSecondsRealtime(0.1f);

        // stage highscores addded to list
        foreach (var stage in stages)
        {
            highScores.Add(stage.highscore);
            highCombos.Add(stage.highcombo);
        }
    }

    public void OnReturnClicked()
    {
        SceneManager.LoadScene("GameMenu");
    }

    public void UpdateSidePanel(int stageSelected)
    {
        buttonSound.Play(); // play sfx

        songText.text = stages[stageSelected].title;
        artistText.text = stages[stageSelected].artist;
        difficultyText.text = stages[stageSelected].difficulty;
        lengthText.text = "Length: " + stages[stageSelected].length;
        bpmText.text = "BPM: " + stages[stageSelected].bpm.ToString();
        highScoreText.text = "High Score: " + stages[stageSelected].highscore.ToString();
        highComboText.text = "Best Combo: " + stages[stageSelected].highcombo.ToString() + " / " + stages[stageSelected].maxCombo.ToString();

        iconImage.sprite = stages[stageSelected].icon;
    }

    public static void setHighscore(int stageID, int highscore, int highcombo)
    {
        // set highscore
        StageManager.Instance.highScores[stageID] = highscore;
        StageManager.Instance.highCombos[stageID] = highcombo;

        // save all highscores
        StageManager.Instance.Save();
    }

    public void Save()
    {
        // passes a referance to the stage manager class
        SaveSystem.SaveHighScoreData(StageManager.Instance);
    }
}
