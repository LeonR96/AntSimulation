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
        int i;
        int j;

        for (int antIdx = 0; antIdx < antsCoordinates.Count; antIdx++)
        {
            i = (int) antsCoordinates[antIdx].x;
            j = (int) antsCoordinates[antIdx].y;

            texture.SetPixel(i, j, COLOR.ant);
        }

        texture.SetPixel(CONST.homeCoordinates.x, CONST.homeCoordinates.y, COLOR.home);
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

        // Apply updated texture
        texture.Apply();
    }
}
