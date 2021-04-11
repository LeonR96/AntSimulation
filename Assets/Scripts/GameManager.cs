using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class CONST
{
    public static int width = 128;
    public static int height = 128;

    public static Color antColor = Color.white;
    public static Color resourceColor = Color.yellow;
}

public struct Ant
{
    public Vector2 coordinates;
    public Vector2 direction;
};

public class GameManager : MonoBehaviour
{
    public AntManager antManager;
    public ResourceManager resourceManager;
    public SpriteRenderer spriteRenderer;

    private Texture2D texture;
    private float updatePeriod = 0.01f;
    private float lastUpdateTimestamp;

    // Start is called before the first frame update
    void Start()
    {
        InitializeTexture();

        antManager.InitializeAnts();

        resourceManager.InitializeResources(texture);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > (lastUpdateTimestamp + updatePeriod))
        {
            UpdateTexture();
        }
    }

    private void InitializeTexture()
    {
        Texture2D newTexture = new Texture2D(CONST.width,
                                             CONST.height);
        Sprite sprite = Sprite.Create(newTexture,
                                      new Rect(0, 0, CONST.width, CONST.height),
                                      new Vector2(0.5f, 0.5f));

        // Draw all pixels black
        for (int i = 0; i < newTexture.width; i++)
        {
            for (int j = 0; j < newTexture.height; j++)
            {
                newTexture.SetPixel(i, j, Color.black);
            }
        }

        // Apply texture and set sprite
        newTexture.Apply();
        spriteRenderer.sprite = sprite;
        texture = newTexture;

        // Set initial update timestamp
        lastUpdateTimestamp = Time.time;
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

    private void UpdateTexture()
    {
        // Store old texture as a baseline for the update
        Texture2D oldTexture = StoreTexture();

        // Blur old texture
        BlurTexture(oldTexture);

        // Update ant location on the texture
        antManager.UpdateAnts(texture);
    }
}
