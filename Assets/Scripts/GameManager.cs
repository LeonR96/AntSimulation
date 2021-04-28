using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class CONST
{
    public static int width = 128;
    public static int height = width;
    public static Vector2Int homeCoordinates = new Vector2Int(width / 2, height / 2);

    public static int bluringRay = 0;
    public static float bluringFactor = 0.9f;
    public static float frameEvaporation = 0.1f;

    public static float almostZero = 1.0E-6f;

    public static float deltaTime = 0.0f;
}

static class ANT
{
    public static float yawRate = 90.0f;

    public static float randomizationPeriodMin = 2.0f;
    public static float randomizationPeriodMax = 3.0f;
}

static class COLOR
{
    public static Color empty = Color.black;
    public static Color ant = Color.white;
    public static Color pheroToFood = Color.red;
    public static Color pheroToHome = Color.blue;
    public static Color resource = Color.yellow;
    public static Color home = Color.green;
}

public struct Ant
{
    public Vector2 coordinates;
    public Vector2 direction;
    public Vector2 intention;
    public bool hasResource;
    public float nextRandomizationTimestamp;
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

        lastUpdateTimestamp = Time.time;
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
