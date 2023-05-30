using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BoidManager : MonoBehaviour
{
    BoidSpawner spawner;

    [SerializeField]
    Boid boidPrefab;

    [SerializeField]
    int maxNumBoids = 200;
    public float maxCohesionSearchRange = 2.0f;
    public float maxAlignSearchRange = 2.0f;
    public float maxSeparationSearchRange = 1.0f;
    public float maxBoidSpeed = 5.0f;
    public float minBoidSpeed = 0.0f;
    public float maxForce = 20.0f; // maximum magnitude of acceleration vector
    public float alignWeight = 0.0f;
    public float cohesionWeight = 1.0f;
    public float separationWeight = 1.0f;

    [SerializeField]
    float boidScale = 0.2f;

    [HideInInspector]
    public int numBoids = 0;
    public List<Boid> boids = new List<Boid>();

    public bool CanSpawnNewBoid()
    {
        return numBoids < maxNumBoids;
    }
    public void SpawnBoid()
    {
        if (CanSpawnNewBoid())
        {
            Boid newBoid = Instantiate(boidPrefab);
            newBoid.transform.position = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            //newBoid.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0.0f, 360.0f));
            newBoid.transform.localScale = new Vector3(boidScale, boidScale, boidScale);
            newBoid.transform.up = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);

            boids.Add(newBoid);
            boids[numBoids].Initialize(numBoids);
            ++numBoids;
            print("spawn a boid");
        }
    }
}
