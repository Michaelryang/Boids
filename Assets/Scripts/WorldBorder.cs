using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBorder : MonoBehaviour
{
    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    // Start is called before the first frame update
    void Start()
    {
        BoxCollider bc = GetComponent<BoxCollider>();
        Vector3 center = new Vector3(bc.center.x, bc.center.y, 0.0f);
        Vector3 worldPos = transform.TransformPoint(bc.center);
        minX = worldPos.x - bc.size.x * 0.5f;
        maxX = worldPos.x + bc.size.x * 0.5f;
        minY = worldPos.y - bc.size.y * 0.5f;
        maxY = worldPos.y + bc.size.y * 0.5f;
    }

    public bool isOutOfBounds(Transform t)
    {
        return isOutOfBounds(t.position);
    }

    public bool isOutOfBounds(Vector3 pos)
    {
        return pos.x > maxX || pos.x < minX || pos.y > maxY || pos.y < minY;
    }

    public Vector3 getNewPosition(Transform t)
    {
        return getNewPosition(t.position);
    }

    public Vector3 getNewPosition(Vector3 pos)
    {
        if (pos.x > maxX)
        {
            pos = new Vector3(minX, pos.y, pos.z);
        }
        if (pos.x < minX)
        {
            pos = new Vector3(maxX, pos.y, pos.z);
        }
        if (pos.y > maxY)
        {
            pos = new Vector3(pos.x, minY, pos.z);
        }
        if (pos.y < minY)
        {
            pos = new Vector3(pos.x, maxY, pos.z);
        }

        return pos;
    }
}
