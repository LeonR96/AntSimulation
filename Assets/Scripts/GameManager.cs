using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class CONST
{
    public static int width = 128;
    public static int height = width;

    public static float bluringFactor = 0.9f;

    public static Color antColor = Color.white;
    public static Color resourceColor = Color.yellow;
    public static Color homeColor = Color.red;

    public static float antYawRate = 180.0f * Mathf.Deg2Rad;

    public static float deltaTime = 0.0f;
}

public struct Ant
{
    public Vector2 coordinates;
    public Vector2 direction;
    public Vector2 intention;
    public bool hasResource;
};

public class GameManager : MonoBehaviour
{
    public TextureManager textureManager;
    public AntManager antManager;
    public ResourceManager resourceManager;
    public SpriteRenderer spriteRenderer;

    private float updatePeriod = 0.05f;
    private float lastUpdateTimestamp;

    // Start is called before the first frame update
    void Start()
    {
        InitializeSprite();

        antManager.InitializeAnts();

        resourceManager.InitializeResources();

        AssignManagers();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > (lastUpdateTimestamp + updatePeriod))
        {
            antManager.UpdateAnts();

            textureManager.UpdateTexture();

            // Set initial update timestamp
            CONST.deltaTime = Time.time - lastUpdateTimestamp;
            lastUpdateTimestamp = Time.time;
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
