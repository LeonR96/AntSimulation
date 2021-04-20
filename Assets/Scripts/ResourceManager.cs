using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public int clusterQty;

    private Texture2D resourcesTexture;
    private float clusterRadiusSquare = 100.0f;

    public void InitializeResources()
    {
        Texture2D newTexture = new Texture2D(CONST.width,
                                             CONST.height);
        int clusterIdx;
        List<Vector2Int> clusterCenters = new List<Vector2Int>();
        Vector2Int clusterCenter = new Vector2Int();
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

        // Draw all resource pixels black
        for (int i = 0; i < CONST.width; i++)
        {
            for (int j = 0; j < CONST.height; j++)
            {
                newTexture.SetPixel(i, j, COLOR.empty);
            }
        }

        for (int i = 0; i < CONST.width; i++)
        {
            for (int j = 0; j < CONST.height; j++)
            {
                for (clusterIdx = 0; clusterIdx < clusterQty; clusterIdx++)
                {
                    clusterCenter = clusterCenters[clusterIdx];

                    xOffset = ((float) i) - ((float) clusterCenter.x);
                    yOffset = ((float) j) - ((float) clusterCenter.y);

                    distanceToClusterCenterSquare = xOffset * xOffset + yOffset * yOffset;

                    // Draw resource pixels
                    if (distanceToClusterCenterSquare < clusterRadiusSquare)
                    {
                        newTexture.SetPixel(i, j, COLOR.resource);
                    }
                }
            }
        }

        // Store initialized resources texture
        resourcesTexture = newTexture;
    }

    public Texture2D GetResourcesTexture()
    {
        return resourcesTexture;
    }

    public bool GetResourceAtCoordinates(Vector2 coordinates)
    {
        return (resourcesTexture.GetPixel((int) coordinates.x, (int) coordinates.y) == COLOR.resource);
    }

    public void RemoveResourceAtCoordinates(Vector2 coordinates)
    {
        resourcesTexture.SetPixel((int) coordinates.x, (int) coordinates.y, COLOR.empty);
    }
}
