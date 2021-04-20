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

    private Texture2D StoreTexture()
    {
        Texture2D oldTexture = new Texture2D(CONST.width, CONST.height);

        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                oldTexture.SetPixel(i, j, texture.GetPixel(i, j));
            }
        }

        return oldTexture;
    }

    private void BlurTexture()
    {
        Texture2D oldTexture = StoreTexture();
        Color pixelColor;
        Color sidePixelColor;
        int binQty;
        float colorScaling;

        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                pixelColor = oldTexture.GetPixel(i, j);

                if (pixelColor != COLOR.resource)
                {
                    binQty = 1;

                    // Use pixel below if it exists
                    if (i != 0)
                    {
                        sidePixelColor = oldTexture.GetPixel(i - 1, j);

                        if (sidePixelColor != COLOR.resource)
                        {
                            pixelColor += sidePixelColor;
                            binQty++;
                        }

                        // Use pixel on top if it exists
                        if (i != (texture.height - 1))
                        {
                            sidePixelColor = oldTexture.GetPixel(i + 1, j);

                            if (sidePixelColor != COLOR.resource)
                            {
                                pixelColor += sidePixelColor;
                                binQty++;
                            }
                        }
                    }

                    // Use pixel to the left if it exists
                    if (j != 0)
                    {
                        sidePixelColor = oldTexture.GetPixel(i, j - 1);

                        if (sidePixelColor != COLOR.resource)
                        {
                            pixelColor += sidePixelColor;
                            binQty++;
                        }

                        // Use pixel right if it exists
                        if (j != (texture.width - 1))
                        {
                            sidePixelColor = oldTexture.GetPixel(i, j + 1);

                            if (sidePixelColor != COLOR.resource)
                            {
                                pixelColor += sidePixelColor;
                                binQty++;
                            }
                        }
                    }

                    // Compute scaling factor
                    colorScaling = CONST.bluringFactor / ((float) binQty);

                    // Mean and decay pixel color
                    pixelColor.r *= colorScaling;
                    pixelColor.g *= colorScaling;
                    pixelColor.b *= colorScaling;
                }

                // Set pixel
                texture.SetPixel(i, j, pixelColor);
            }
        }
    }

    private void DrawPheromones()
    {
        Texture2D pheromoneTexture = antManager.GetPheromoneTexture();

        for (int i = 0; i < CONST.width; i++)
        {
            for (int j = 0; j < CONST.height; j++)
            {
                texture.SetPixel(i, j, pheromoneTexture.GetPixel(i, j));
            }
        }
    }

    private void DrawResources()
    {
        Texture2D resourcesTexture = resourceManager.GetResourcesTexture();

        for (int i = 0; i < CONST.width; i++)
        {
            for (int j = 0; j < CONST.height; j++)
            {
                texture.SetPixel(i, j, texture.GetPixel(i, j) + resourcesTexture.GetPixel(i, j));
            }
        }
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
        // Draw pheromones
        DrawPheromones();

        // Draw resources
        DrawResources();

        // Draw ants
        DrawAnts();

        // Apply updated texture
        texture.Apply();
    }
}
