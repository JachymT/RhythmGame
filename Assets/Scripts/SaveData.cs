using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public float musicVolume;
    public float sfxVolume;
    public float noteSpeed;

    // constructor method
    public GameData(Game game)
    {
        this.musicVolume = game.musicSlider.value;
        this.sfxVolume = game.sfxSlider.value;
        this.noteSpeed = game.speedSlider.value;
    }
}

[System.Serializable]
public class ScoreData
{
    public int[] highScores  = new int[5];
    public int[] highCombos  = new int[5];

    public ScoreData(StageManager stageManager)
    {
        for (int i = 0 ; i < 5; i++)
        {
            this.highScores[i] = stageManager.highScores[i];
            this.highCombos[i] = stageManager.highCombos[i];
        }
    }
}
