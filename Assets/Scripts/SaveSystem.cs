using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

// static classes cannot be instantiated
public static class SaveSystem
{
    private static string gameDataPath = "RhythmGameData.bin";
    private static string highScoreDataPath = "ScoreData.bin";

    // saves user game data to file
    public static void SaveGameData(Game game)
    {
        string path = Path.Combine(Application.persistentDataPath, gameDataPath);

        // converts data to a serilazable object
        GameData data = new GameData(game);

        // write to new file
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    // reads file and returns serializable game data object
    public static GameData LoadGameData()
    {
        string path = Path.Combine(Application.persistentDataPath, gameDataPath);

        //check for file
        if (File.Exists(path))
        {
            Debug.Log("game data file found");

            // read from file 
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            GameData data = formatter.Deserialize(stream) as GameData; // cast type
            stream.Close();
            return data;

        }
        else
        {
            Debug.Log("game data file not found");
            return null;
        }
    }

    // saves user highscore data to file
    public static void SaveHighScoreData(StageManager stageManager)
    {
        string path = Path.Combine(Application.persistentDataPath, highScoreDataPath);
        // converts data to a serilazable object
        ScoreData data = new ScoreData(stageManager);

        // write to new file
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    // reads file and returns serializable highscore data object
    public static ScoreData LoadHighScoreData()
    {
        string path = Path.Combine(Application.persistentDataPath, highScoreDataPath);

        //check for file
        if (File.Exists(path))
        {
            Debug.Log("high score file found");

            // read from file 
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            ScoreData data = formatter.Deserialize(stream) as ScoreData; // cast type
            stream.Close();
            return data;

        }
        else
        {
            Debug.Log("high score file not found");
            return null;
        }
    }
}
