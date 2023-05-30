using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    WorldBorder border;
    BoidManager bm;

    public float maxForce = 10.0f;

    Transform newTransform;

    
    int neighborsInCohesionRadius = 0;
    [SerializeField]
    int neighbors = 0;
    [SerializeField]
    bool drawDebug = false;
    public int myID;

    public Vector3 flockHeading;
    public Vector3 flockCenter;
    public Vector3 separationHeading;

    public Vector3 acceleration;

    public Vector3 velocity;
    public Vector3 position;
    public Vector3 up; // this tracks the heading

    private void OnDrawGizmos() // debug lines to help visualize 
    {
        if (drawDebug)
        {
            Gizmos.color = new Color(122.0f / 255, 255.0f / 255, 158.0f / 255, 1);
            Gizmos.DrawWireSphere(transform.position, bm.maxCohesionSearchRange);

            Gizmos.color = new Color(255.0f / 255, 129.0f / 255, 122.0f / 255, 1);
            Gizmos.DrawWireSphere(transform.position, bm.maxSeparationSearchRange);

            for (int x = 0; x < bm.boids.Count; ++x)
            {
                Vector3 neighbor = bm.boids[x].getPosition();
                float distanceToNeighbor = Vector3.Distance(transform.position, neighbor);

                if (distanceToNeighbor < bm.maxSeparationSearchRange)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(transform.position, bm.boids[x].getPosition());
                }
                else if (distanceToNeighbor < bm.maxCohesionSearchRange)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(transform.position, bm.boids[x].getPosition());
                }
            }

            Gizmos.color = Color.cyan;
            if (neighborsInCohesionRadius > 0)
            {
                Gizmos.DrawWireSphere(flockCenter, 0.2f);
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, velocity + transform.position);
        }
    }

    public Vector3 getPosition()
    {
        return transform.position;
    }

    public void Initialize(int id)
    {
        myID = id;

        border = GameObject.FindObjectOfType(typeof(WorldBorder)) as WorldBorder;
        bm = GameObject.FindObjectOfType(typeof(BoidManager)) as BoidManager;

        newTransform = transform;
        up = newTransform.up;
        position = newTransform.position;

        velocity = transform.up * bm.maxBoidSpeed;
    }

    Vector3 GetAcceleration(Vector3 direction)
    {
        return Vector3.ClampMagnitude(direction.normalized * bm.maxBoidSpeed - velocity, maxForce);
    }

    // Update is called once per frame
    void Update()
    {
        maxForce = bm.maxForce;
        position = transform.position;
        up = transform.up;
        neighborsInCohesionRadius = 0;
        neighbors = 0;

        acceleration = Vector3.zero;
        flockCenter = Vector3.zero;
        flockHeading = Vector3.zero;
        separationHeading = Vector3.zero;

        // calculate data
        for (int x = 0; x < bm.numBoids; ++x)
        {
            Boid neighbor = bm.boids[x];


            if (neighbor.myID != myID)
            {
                Vector3 offset = neighbor.position - position;
                float distanceSquared = Vector3.Distance(neighbor.position, position);
                distanceSquared *= distanceSquared;

                float alignSearchRadiusSquared = bm.maxAlignSearchRange * bm.maxAlignSearchRange;
                float cohesionSearchRadiusSquared = bm.maxCohesionSearchRange * bm.maxCohesionSearchRange;
                float separationSearchRadiusSquared = bm.maxSeparationSearchRange * bm.maxSeparationSearchRange;
                bool isNeighbor = false;

                if (distanceSquared < alignSearchRadiusSquared)
                {
                    flockHeading += neighbor.up;
                    isNeighbor = true;
                }

                if (distanceSquared < cohesionSearchRadiusSquared)
                {
                    neighborsInCohesionRadius++;
                    flockCenter += neighbor.position;
                    isNeighbor = true;
                }

                if (distanceSquared < separationSearchRadiusSquared)
                {
                    separationHeading -= offset / distanceSquared;
                    isNeighbor = true;
                }

                if (isNeighbor)
                {
                    ++neighbors;
                }
            }
        }

        // calculate accel
        if (neighbors > 0)
        {
            Vector3 cohes = Vector3.zero;
            Vector3 separ = Vector3.zero;
            Vector3 align = Vector3.zero;

            if (neighborsInCohesionRadius > 0)
            {
                flockCenter /= neighborsInCohesionRadius;

                Vector3 offsetToCenter = flockCenter - position;
                cohes = GetAcceleration(offsetToCenter) * bm.cohesionWeight;
            }

            separ = GetAcceleration(separationHeading) * bm.separationWeight;
            align = GetAcceleration(flockHeading) * bm.alignWeight;

            acceleration += cohes;
            acceleration += separ;
            acceleration += align;
        }
        
        // change velocity heading, and clamp to min/max speed
        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp(speed, bm.minBoidSpeed, bm.maxBoidSpeed);
        velocity = dir * speed;

        // set new tranform
        newTransform.position += velocity * Time.deltaTime;
        newTransform.up = dir;
        position = newTransform.position;
        up = dir;

        transform.position = newTransform.position;
        transform.up = newTransform.up;

        // teleport if out of bounds
        if (border.isOutOfBounds(transform))
        {
            transform.position = border.getNewPosition(transform.position);
        }
    }
}
