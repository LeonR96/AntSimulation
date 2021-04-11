using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public int clusterQty;

    private float clusterRadiusSquare = 100.0f;

    public void InitializeResources(Texture2D texture)
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
            for (j = 0; j < CONST.height; j++)
            {
                for (clusterIdx = 0; clusterIdx < clusterQty; clusterIdx++)
                {
                    clusterCenter = clusterCenters[clusterIdx];

                    xOffset = ((float) i) - ((float) clusterCenter.x);
                    yOffset = ((float) j) - ((float) clusterCenter.y);

                    distanceToClusterCenterSquare = xOffset * xOffset + yOffset * yOffset;

                    // Color resource pixels
                    if (distanceToClusterCenterSquare < clusterRadiusSquare)
                    {
                        texture.SetPixel(i, j, CONST.resourceColor);
                    }
                }
            }
        }

        texture.Apply();
    }
}
