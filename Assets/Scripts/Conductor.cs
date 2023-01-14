using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using System;
using TMPro;

public class Conductor : MonoBehaviour
{
    public static Conductor Instance;
    public static MidiFile midiFile;

    public Lane[] lanes; // array
    public static float songDelay; // in milliseconds
    public double marginOfError; // in milliseconds
    public float inputDelay; // in milliseconds
    public static string fileName;
    public static float noteTime; // travel time aka aproach rate
    public float noteSpawnX;
    public float noteTapX;
    public float noteDespawnX;
    public AudioSource audioSource;
    public static AudioClip currentSong;
    public static float volumeMusic;
    public float songBeat;
    public static float bpm;
    private float secondsPerBeat
    {
        get
        {
            return 60f / bpm;
        }
    }
    public bool beatFrame;
    private float audioStartTime;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;


        if (Application.streamingAssetsPath.StartsWith("http://") || Application.streamingAssetsPath.StartsWith("https://"))
        {
            // for web gl build
            StartCoroutine(ReadFromWeb());
        }
        else
        {
            // for playing the game locally
            ReadFromFile();
        }
    }

    private IEnumerator ReadFromWeb()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + fileName))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                byte[] results = www.downloadHandler.data;
                using (var stream = new MemoryStream(results))
                {
                    midiFile = MidiFile.Read(stream);
                    GetDataFromMidi();
                }
            }
        }
    }

    // File name specified beforehand by the stageManager class
    private bool ReadFromFile()
    {
        // check if midi file exists in the streaming assets folder
        if (File.Exists(Application.streamingAssetsPath + "/" + fileName)){
            // read from midi file
            midiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + fileName);
            GetDataFromMidi();
            return true;
        }
        // if midi file does not exist
        else{
            print("File not found");
            return false;
        }
    }

    // Using the Dry Wet Midi library
    public void GetDataFromMidi()
    {
        // notes read and copied to array
        var notes = midiFile.GetNotes();
        var array = new Melanchall.DryWetMidi.Interaction.Note[notes.Count];
        notes.CopyTo(array, 0);

        // passes array to lane classes
        foreach (var lane in lanes) 
            lane.SetTimeStamps(array);

        // calls start song after waiting for songDelay miliseconds
        StartSong();
    }

    public void StartSong()
    {
        audioStartTime = (float)AudioSettings.dspTime;
        audioSource.volume = volumeMusic;
        audioSource.clip = currentSong;
        audioSource.Play();
    }

    public void StopSong()
    {
        audioSource.Stop();
    }


    public static double GetAudioSourceTime()
    {
        // replacesd time sampling method with dspTime to allow for build in song delay
        return (double)(AudioSettings.dspTime - Instance.audioStartTime - songDelay/1000);
    }

    // redundant, counts beats during the song
    // void Update()
    // {
    //     Instance.beatFrame = false;
    //     if ((GetAudioSourceTime() / Instance.secondsPerBeat) - Instance.songBeat > 1)
    //     {
    //         Instance.beatFrame = true;
    //         Instance.songBeat = Convert.ToInt32(GetAudioSourceTime() / Instance.secondsPerBeat);
    //     }
    // }
}
