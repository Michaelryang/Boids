using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoidSpawner : MonoBehaviour
{
    public BoidManager bm;

    [SerializeField]
    private float boidSpawnInterval = 30.0f; // in ms
    private float boidSpawnIntervalCounter = 0.0f;

    void Start()
    {
        bm = GameObject.FindObjectOfType(typeof(BoidManager)) as BoidManager;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name == "BoidSpawner")
                {
                    boidSpawnIntervalCounter += Time.deltaTime;

                    if (boidSpawnIntervalCounter > boidSpawnInterval)
                    {
                        boidSpawnIntervalCounter = 0;

                        bm.SpawnBoid();
                    }
                }
            }
        }
        else 
        {
            boidSpawnIntervalCounter = 0;
        }
    }
}