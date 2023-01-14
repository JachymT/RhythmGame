using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;
    public KeyCode input1;
    public KeyCode input2;
    public KeyCode input3;
    List<Note> notes = new List<Note>();
    public List<double> timeStamps = new List<double>();

    public GameObject notePrefab;

    public SpriteRenderer laneHitArea;

    private float hitAreaAlpha;
    private float grey = 0.5f; // alpha value constant
    private float t; // for interpolation

    int spawnIndex = 0;
    int inputIndex = 0;

    void Start()
    {
        // between the two lanes, sets allNotes to the numebr of total notes 
        ScoreManager.Instance.allNotes = ScoreManager.Instance.allNotes + timeStamps.Count;

        // set hit area colour
        laneHitArea.color = new Color(1f,1f,1f,grey);  
    }

    public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] array)
    {
        foreach (var note in array)
        {
            // filters notes
            if (note.NoteName == noteRestriction)
            {
                // converts to metric time
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, Conductor.midiFile.GetTempoMap());

                // convert to seconds
                var secondsTimeSpan = (double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f;

                // adds to timeStamps list if not duplicate
                if(!(timeStamps.Contains(secondsTimeSpan))){
                    timeStamps.Add(secondsTimeSpan);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // spawn notes if there are notes left to spawn and time is correct
        if (spawnIndex < timeStamps.Count && !ScoreManager.Instance.failedControl)
        {
            if (Conductor.GetAudioSourceTime() >= timeStamps[spawnIndex] - Conductor.noteTime)
            {
                // spawn note
                var note = Instantiate(notePrefab, transform);
                notes.Add(note.GetComponent<Note>());

                // calculate how late note is spawning to adujst stating value of t
                double timeDelay = Conductor.GetAudioSourceTime() - (timeStamps[spawnIndex] - Conductor.noteTime);
                note.GetComponent<Note>().spawnDelay = timeDelay / (Conductor.noteTime * 1.25);
        
                // assign time
                note.GetComponent<Note>().assignedTime = (float)timeStamps[spawnIndex];
                spawnIndex++;
            }
        }

        // register inputs
        if (inputIndex < timeStamps.Count && !ScoreManager.Instance.failedControl)
        {
            double timeStamp = timeStamps[inputIndex]; // hit time of incoming note
            double marginOfError = (Conductor.Instance.marginOfError) / 1000; // convert marginOfError to seconds
            double audioTime = Conductor.GetAudioSourceTime() - (Conductor.Instance.inputDelay / 1000.0f); // convert input delay to seconds

            // Input.GetKeyDown(input) || Conductor.Instance.beatFrame == true
            // Input.GetKeyDown(input) || Math.Abs(timeStamp - audioTime) < 0.01
            if (Input.GetKeyDown(input1) || Input.GetKeyDown(input2) || Input.GetKeyDown(input3))
            {
                // hit area effects starts at full alpha brightness
                hitAreaAlpha = 1f;
                t = 1f;

                // conditions to hit a perfect
                if (Math.Abs(audioTime - timeStamp) < marginOfError)
                {
                    Hit(0);
                    //print($"Hit note {inputIndex} with PERFECT timing and {audioTime - timeStamp} delay");
                    Destroy(notes[inputIndex].gameObject);
                    inputIndex++;
                }

                else if (Math.Abs(audioTime - timeStamp) < (marginOfError*2.25)) //muse dash uses *2.6 and a 100ms margin of error, 
                { 
                    Hit(1);
                    //print($"Hit note {inputIndex} with GOOD timing and {audioTime - timeStamp} delay");
                    Destroy(notes[inputIndex].gameObject);
                    inputIndex++;
                }

                //outside of good range (miss range)
                else if (Math.Abs(audioTime - timeStamp) < marginOfError*2.75)
                {
                    Miss();
                    //print($"Missed note {inputIndex} with {audioTime - timeStamp} delay");
                    Destroy(notes[inputIndex].gameObject);
                    inputIndex++;
                }
            }

            // outside of miss range and no input key pressed
            if (timeStamp + marginOfError*2.75 <= audioTime)
            {
                Miss();
                //print($"Missed {inputIndex} note: {audioTime - timeStamp} delay");
                inputIndex++;
            }
        }

        // whilst image isnt at the grey constant, decrease alpha 
        if (hitAreaAlpha != grey)
        {
            fadeHitArea();
        }
    }

    public void fadeHitArea()
    {
        // using unity's non linnear interpolation
        t -= Time.deltaTime/3;
        hitAreaAlpha = Mathf.SmoothStep(grey, hitAreaAlpha, t);

        // change alpha after interpolatinng
        laneHitArea.color = new Color(1f,1f,1f,hitAreaAlpha);  
    }

    // calls scoreManager
    private void Hit(int timingRating)
    {
        ScoreManager.Hit(timingRating);
    }

    // calls scoreManager
    private void Miss()
    {
        ScoreManager.Miss();
    }
}
