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
        for (int i = 0; i < newTexture.width; i++)
        {
            for (int j = 0; j < newTexture.height; j++)
            {
                newTexture.SetPixel(i, j, Color.black);
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
        int i;
        int j;

        for (i = 0; i < texture.width; i++)
        {
            for (j = 0; j < texture.height; j++)
            {
                oldTexture.SetPixel(i, j, texture.GetPixel(i, j));
            }
        }

        return oldTexture;
    }

    private void BlurTexture()
    {
        Texture2D oldTexture = StoreTexture();
        int i;
        int j;
        Color pixelColor;
        Color sidePixelColor;
        int binQty;
        float colorScaling;

        for (i = 0; i < texture.width; i++)
        {
            for (j = 0; j < texture.height; j++)
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

    private void DrawResources()
    {
        List<Vector2Int> resourcesCoordinates = resourceManager.GetResourcesCoordinates();
        int resourceIdx;
        int i;
        int j;

        for (resourceIdx = 0; resourceIdx < resourcesCoordinates.Count; resourceIdx++)
        {
            i = resourcesCoordinates[resourceIdx].x;
            j = resourcesCoordinates[resourceIdx].y;

            texture.SetPixel(i, j, COLOR.resource);
        }
    }

    private void DrawAnts()
    {
        List<Vector2> antsCoordinates = antManager.GetAntsCoordinates();
        int antIdx;
        int i;
        int j;
        Vector2Int homeCoordinates = antManager.GetHomeCoordinates();

        for (antIdx = 0; antIdx < antsCoordinates.Count; antIdx++)
        {
            i = (int) antsCoordinates[antIdx].x;
            j = (int) antsCoordinates[antIdx].y;

            texture.SetPixel(i, j, COLOR.ant);
        }

        texture.SetPixel(homeCoordinates.x, homeCoordinates.y, COLOR.home);
    }

    public void UpdateTexture()
    {
        // Blur texture
        BlurTexture();

        // Draw resources
        DrawResources();

        // Draw ants
        DrawAnts();

        // Apply updated texture
        texture.Apply();
    }
}
