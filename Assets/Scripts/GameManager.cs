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
    public bool hasResource;
};

public class GameManager : MonoBehaviour
{
    public TextureManager textureManager;
    public AntManager antManager;
    public ResourceManager resourceManager;
    public SpriteRenderer spriteRenderer;

    private float updatePeriod = 0.01f;
    private float lastUpdateTimestamp;

    // Start is called before the first frame update
    void Start()
    {
        InitializeSprite();

        antManager.InitializeAnts();

        resourceManager.InitializeResources();

        AssignManagers();

        // Set initial update timestamp
        lastUpdateTimestamp = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > (lastUpdateTimestamp + updatePeriod))
        {
            antManager.UpdateAnts();

            textureManager.UpdateTexture();
        }
    }

    private void InitializeSprite()
    {
        textureManager.InitializeTexture();

        Sprite sprite = Sprite.Create(textureManager.GetTexture(),
                                      new Rect(0, 0, CONST.width, CONST.height),
                                      new Vector2(0.5f, 0.5f));

        spriteRenderer.sprite = sprite;
    }

    private void AssignManagers()
    {
        antManager.resourceManager = resourceManager;

        textureManager.antManager = antManager;
        textureManager.resourceManager = resourceManager;
    }
}
