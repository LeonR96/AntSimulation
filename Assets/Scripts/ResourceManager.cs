using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public int clusterQty;

    private float clusterRadiusSquare = 100.0f;
    private List<List<bool>> resourceStatuses = new List<List<bool>>();

    public void InitializeResources()
    {
        int clusterIdx;
        List<Vector2Int> clusterCenters = new List<Vector2Int>();
        Vector2Int clusterCenter = new Vector2Int();
        int i;
        int j;
        float xOffset;
        float yOffset;
        float distanceToClusterCenterSquare;

        // Create random clusters
        for (clusterIdx = 0; clusterIdx < clusterQty; clusterIdx++)
        {
            clusterCenter.x = Random.Range(0, CONST.width);
            clusterCenter.y = Random.Range(0, CONST.height);

            clusterCenters.Add(clusterCenter);
        }

        for (i = 0; i < CONST.width; i++)
        {
            List<bool> newResourceStatuses = new List<bool>();

            for (j = 0; j < CONST.height; j++)
            {
                newResourceStatuses.Add(false);
            }

            resourceStatuses.Add(newResourceStatuses);
        }

        for (i = 0; i < CONST.width; i++)
        {
            for (j = 0; j < CONST.height; j++)
            {
                for (clusterIdx = 0; clusterIdx < clusterQty; clusterIdx++)
                {
                    clusterCenter = clusterCenters[clusterIdx];

                    xOffset = ((float) i) - ((float) clusterCenter.x);
                    yOffset = ((float) j) - ((float) clusterCenter.y);

                    distanceToClusterCenterSquare = xOffset * xOffset + yOffset * yOffset;

                    // Store resource pixels
                    if (distanceToClusterCenterSquare < clusterRadiusSquare)
                    {
                        resourceStatuses[i][j] = true;
                    }
                }
            }
        }
    }

    public List<Vector2Int> GetResourcesCoordinates()
    {
        List<Vector2Int> resourcesCoordinates = new List<Vector2Int>();
        int i;
        int j;

        for (i = 0; i < CONST.width; i++)
        {
            for (j = 0; j < CONST.height; j++)
            {
                if (resourceStatuses[i][j] == true)
                {
                    resourcesCoordinates.Add(new Vector2Int(i, j));
                }
            }
        }

        return resourcesCoordinates;
    }

    public bool GetResourceAtCoordinates(Vector2 coordinates)
    {
        return resourceStatuses[(int) coordinates.x][(int) coordinates.y];
    }

    public void RemoveResourceAtCoordinates(Vector2 coordinates)
    {
        resourceStatuses[(int) coordinates.x][(int) coordinates.y] = false;
    }
}
