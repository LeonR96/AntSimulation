using System.Collections.Generic;
using UnityEngine;

static class CONST
{
    public const int width = 128;
    public const int height = 128;
}

struct Ant
{
    public Vector2 coordinates;
    public Vector2 direction;
};

public class AntManager : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public int antQty;

    private float updatePeriod = 0.01f;
    private float lastUpdateTimestamp;
    private Texture2D texture;
    private List<Ant> ants = new List<Ant>();

    // Start is called before the first frame update
    void Start()
    {
        SetTexture();

        SetAnts();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > (lastUpdateTimestamp + updatePeriod))
        {
            UpdateTexture();

            lastUpdateTimestamp = Time.time;
        }
    }

    private void SetTexture()
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

    private void SetAnts()
    {
        for (int antIdx = 0 ; antIdx < antQty; antIdx++)
        {
            Ant newAnt = new Ant();

            newAnt.coordinates = new Vector2(Random.Range(0.0f, (float) texture.width), Random.Range(0.0f, (float) texture.height));
            newAnt.direction = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;

            ants.Add(newAnt);
        }
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
        int binQty;

        for (i = 0; i < texture.width; i++)
        {
            for (j = 0; j < texture.height; j++)
            {
                pixelColor = oldTexture.GetPixel(i, j);
                binQty = 1;

                // Use pixel below if it exists
                if (i != 0)
                {
                    pixelColor += oldTexture.GetPixel(i - 1, j);
                    binQty++;
                }

                // Use pixel on top if it exists
                if (i != (texture.height - 1))
                {
                    pixelColor += oldTexture.GetPixel(i + 1, j);
                    binQty++;
                }

                // Use pixel to the left if it exists
                if (j != 0)
                {
                    pixelColor += oldTexture.GetPixel(i, j - 1);
                    binQty++;
                }

                // Use pixel right if it exists
                if (j != (texture.width - 1))
                {
                    pixelColor += oldTexture.GetPixel(i, j + 1);
                    binQty++;
                }

                // Get mean pixel color
                pixelColor.r /= ((float) binQty);
                pixelColor.g /= ((float) binQty);
                pixelColor.b /= ((float) binQty);

                // Decay color
                pixelColor.r *= 0.9f;
                pixelColor.g *= 0.9f;
                pixelColor.b *= 0.9f;

                // Set pixel
                texture.SetPixel(i, j, pixelColor);
            }
        }
    }

    private void UpdateAnts()
    {
        int antIdx;
        Ant ant;

        for (antIdx = 0; antIdx < ants.Count; antIdx++)
        {
            ant = ants[antIdx];

            // Randomize ant direction one it reaches an edge
            if (ant.coordinates.x < 1.0f)
            {
                ant.direction.x = Random.Range(0.0f, 1.0f);
                ant.direction.Normalize();
            }
            else if (ant.coordinates.x > ((float) texture.width - 2.0f))
            {
                ant.direction.x = Random.Range(0.0f, -1.0f);
                ant.direction.Normalize();
            }

            if (ant.coordinates.y < 1.0f)
            {
                ant.direction.y = Random.Range(0.0f, 1.0f);
                ant.direction.Normalize();
            }
            else if (ant.coordinates.y > ((float) texture.height - 2.0f))
            {
                ant.direction.y = Random.Range(0.0f, -1.0f);
                ant.direction.Normalize();
            }

            // Move then update ant
            ant.coordinates += ant.direction;
            ants[antIdx] = ant;

            // Draw ant
            texture.SetPixel((int) ant.coordinates.x, (int) ant.coordinates.y, Color.white);
        }
    }

    private void UpdateTexture()
    {
        // Store old texture as a baseline for the update
        Texture2D oldTexture = StoreTexture();

        // Blur old texture
        BlurTexture(oldTexture);

        // Update ants
        UpdateAnts();

        // Apply texture update
        texture.Apply();
    }
}
