using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureManager : MonoBehaviour
{
    public AntManager antManager;

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

    private void BlurTexture(Texture2D oldTexture)
    {
        int i;
        int j;
        Color pixelColor;
        Color sidePixelColor;
        int binQty;

        for (i = 0; i < texture.width; i++)
        {
            for (j = 0; j < texture.height; j++)
            {
                pixelColor = oldTexture.GetPixel(i, j);

                if (pixelColor != CONST.resourceColor)
                {
                    binQty = 1;

                    // Use pixel below if it exists
                    if (i != 0)
                    {
                        sidePixelColor = oldTexture.GetPixel(i - 1, j);

                        if (sidePixelColor != CONST.resourceColor)
                        {
                            pixelColor += sidePixelColor;
                            binQty++;
                        }
                    }

                    // Use pixel on top if it exists
                    if (i != (texture.height - 1))
                    {
                        sidePixelColor = oldTexture.GetPixel(i + 1, j);

                        if (sidePixelColor != CONST.resourceColor)
                        {
                            pixelColor += sidePixelColor;
                            binQty++;
                        }
                    }

                    // Use pixel to the left if it exists
                    if (j != 0)
                    {
                        sidePixelColor = oldTexture.GetPixel(i, j - 1);

                        if (sidePixelColor != CONST.resourceColor)
                        {
                            pixelColor += sidePixelColor;
                            binQty++;
                        }
                    }

                    // Use pixel right if it exists
                    if (j != (texture.width - 1))
                    {
                        sidePixelColor = oldTexture.GetPixel(i, j + 1);

                        if (sidePixelColor != CONST.resourceColor)
                        {
                            pixelColor += sidePixelColor;
                            binQty++;
                        }
                    }

                    // Get mean pixel color
                    pixelColor.r /= ((float) binQty);
                    pixelColor.g /= ((float) binQty);
                    pixelColor.b /= ((float) binQty);

                    // Decay color
                    pixelColor.r *= 0.9f;
                    pixelColor.g *= 0.9f;
                    pixelColor.b *= 0.9f;
                }

                // Set pixel
                texture.SetPixel(i, j, pixelColor);
            }
        }
    }

    private void DrawAnts()
    {
        List<Vector2> antsCoordinates = antManager.GetAntsCoordinates();
        int antIdx;
        int i;
        int j;

        for (antIdx = 0; antIdx < antsCoordinates.Count; antIdx++)
        {
            i = (int) antsCoordinates[antIdx].x;
            j = (int) antsCoordinates[antIdx].y;

            // Set ant resource if it is located on a resource pixel
            if (texture.GetPixel(i, j) == CONST.resourceColor)
            {
                antManager.SetAntResource(antIdx, true);
            }

            texture.SetPixel(i, j, CONST.antColor);
        }
    }

    public void UpdateTexture()
    {
        // Store old texture as a baseline for the update
        Texture2D oldTexture = StoreTexture();

        // Blur old texture
        BlurTexture(oldTexture);

        // Draw ants
        DrawAnts();

        // Apply updated texture
        texture.Apply();
    }
}
