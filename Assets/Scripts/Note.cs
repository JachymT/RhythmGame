using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    private double timeInstantiated;
    public float assignedTime;
    public double spawnDelay;

    void Start()
    {
        // spawn delay moves note left according to how late it was spawned 
        timeInstantiated = Conductor.GetAudioSourceTime() - spawnDelay;
    }

    // Update is called once per frame
    void Update()
    {
        double timeSinceInstantiated = Conductor.GetAudioSourceTime() - timeInstantiated;
        float t = (float)(timeSinceInstantiated / (Conductor.noteTime * 1.25));    // noteTime * 1.25 represents total time to travel from start to end 

        if (!ScoreManager.Instance.failedControl)
        {
            if (t > 1)
            {
                Destroy(gameObject);
            }
            else
            {
                transform.localPosition = Vector3.Lerp(Vector3.right * Conductor.Instance.noteSpawnX, Vector3.right * Conductor.Instance.noteDespawnX, t); 
                GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }
}
