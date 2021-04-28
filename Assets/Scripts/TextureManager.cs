using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureManager : MonoBehaviour
{
    public AntManager antManager;
    public ResourceManager resourceManager;

    private Texture2D texture;

    public void InitializeTexture()
    {
        Texture2D newTexture = new Texture2D(CONST.width,
                                             CONST.height);

        // Draw all pixels black
        for (int i = 0; i < CONST.width; i++)
        {
            for (int j = 0; j < CONST.height; j++)
            {
                newTexture.SetPixel(i, j, COLOR.empty);
            }
        }

        texture = newTexture;
    }

    public Texture2D GetTexture()
    {
        return texture;
    }

    private void DrawAnts()
    {
        List<Vector2> antsCoordinates = antManager.GetAntsCoordinates();
        int iAnt;
        int jAnt;

        for (int antIdx = 0; antIdx < antsCoordinates.Count; antIdx++)
        {
            iAnt = (int) antsCoordinates[antIdx].x;
            jAnt = (int) antsCoordinates[antIdx].y;

            texture.SetPixel(iAnt, jAnt, COLOR.ant);
        }
    }

    private void DrawHome()
    {
        int iHome = CONST.homeCoordinates.x;
        int jHome = CONST.homeCoordinates.y;
        int radiusSquare = CONST.homeRadius * CONST.homeRadius;
        int iMin = Mathf.Max(iHome - CONST.homeRadius, 0);
        int iMax = Mathf.Min(iHome + CONST.homeRadius + 1, CONST.width);
        int jMin = Mathf.Max(jHome - CONST.homeRadius, 0);
        int jMax = Mathf.Min(jHome + CONST.homeRadius + 1, CONST.height);

        for (int i = iMin; i < iMax; i++)
        {
            for (int j = jMin; j < jMax; j++)
            {
                if (antManager.IsHome(i, j) == true)
                {
                    texture.SetPixel(i, j, COLOR.home);
                }
            }
        }
    }

    public void UpdateTexture()
    {
        Texture2D pheromoneTexture = antManager.GetPheromoneTexture();
        Texture2D resourcesTexture = resourceManager.GetResourcesTexture();

        for (int i = 0; i < CONST.width; i++)
        {
            for (int j = 0; j < CONST.height; j++)
            {
                // Draw pheromones, then resources on top of them
                texture.SetPixel(i, j, pheromoneTexture.GetPixel(i, j) + resourcesTexture.GetPixel(i, j));
            }
        }

        // Draw ants
        DrawAnts();

        // Draw home
        DrawHome();

        // Apply updated texture
        texture.Apply();
    }
}
