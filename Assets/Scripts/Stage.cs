using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

// inheritance required required for OnSelect() and OnPointerEnter()
public class Stage : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    public TMPro.TextMeshProUGUI stageText;
    public Image stageIconSmall;
    public int stageID;
    public string midiFile;
    public AudioClip audioFile; 
    public Sprite icon;
    public float bpm;
    public string length;
    public string title;
    public string artist;
    public string difficulty;
    public int highscore;
    public int highcombo;
    public int maxCombo;
    public float audioDelay;

    void Start()
    {
        // button style initialized
        stageText.text = title.ToString() + " - " + artist.ToString();
        stageIconSmall.sprite = icon;

        Load();
    }

    public void Load()
    {
        // gets game data from file
        ScoreData data = SaveSystem.LoadHighScoreData();

        if (data != null)
        {
            // only loads data matching stage ID
            highscore = data.highScores[stageID];
            highcombo = data.highCombos[stageID];
        }
        else
        {
            Debug.Log("No high score save made");
        }
    }

    // button pressed
    public void OnEnter()
    {
        Conductor.fileName = midiFile;
        Conductor.songDelay = audioDelay;
        Conductor.bpm = bpm;
        Conductor.currentSong = audioFile;
        ScoreManager.highscore = highscore;
        ScoreManager.currentStage = stageID;
        SceneManager.LoadScene("MainStage");
    }

    // button selected
    public void OnSelect(BaseEventData eventData)
    {
        StageManager.Instance.UpdateSidePanel(stageID);
    }

    // button hovered over
    public void OnPointerEnter(PointerEventData eventData)
    {
        StageManager.Instance.UpdateSidePanel(stageID);
    }
}
