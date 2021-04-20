using System.Collections.Generic;
using UnityEngine;

public class AntManager : MonoBehaviour
{
    public ResourceManager resourceManager;
    public int antQty;

    private List<Ant> ants = new List<Ant>();
    private Texture2D pheromoneTexture;

    public void InitializeAnts()
    {
        Texture2D newTexture = new Texture2D(CONST.width,
                                             CONST.height);

        for (int antIdx = 0 ; antIdx < antQty; antIdx++)
        {
            Ant newAnt = new Ant();

            newAnt.coordinates = new Vector2(Random.Range(0.0f, (float) CONST.width), Random.Range(0.0f, (float) CONST.height));
            newAnt.intention = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
            newAnt.direction = newAnt.intention;
            newAnt.hasResource = false;

            ants.Add(newAnt);
        }

        // Draw all pheromone pixels black
        for (int i = 0; i < CONST.width; i++)
        {
            for (int j = 0; j < CONST.height; j++)
            {
                newTexture.SetPixel(i, j, COLOR.empty);
            }
        }

        // Store pheromone texture
        pheromoneTexture = newTexture;
    }

    private void BounceAnt(ref Ant ant)
    {
        bool didAntBounce = false;

        // Randomize ant direction if it reaches an edge
        if (ant.coordinates.x < 1.0f)
        {
            ant.direction.x = Random.Range(0.0f, 1.0f);
            didAntBounce = true;
        }
        else if (ant.coordinates.x > ((float) CONST.width - 2.0f))
        {
            ant.direction.x = Random.Range(0.0f, -1.0f);
            didAntBounce = true;
        }

        if (ant.coordinates.y < 1.0f)
        {
            ant.direction.y = Random.Range(0.0f, 1.0f);
            didAntBounce = true;
        }
        else if (ant.coordinates.y > ((float) CONST.height - 2.0f))
        {
            ant.direction.y = Random.Range(0.0f, -1.0f);
            didAntBounce = true;
        }

        // Re-normalize ant direction and reset its intention
        if (didAntBounce == true)
        {
            ant.direction.Normalize();
            ant.intention = ant.direction;
        }
    }

    private void UpdatePheromones()
    {
        Texture2D bluredTexture = new Texture2D(CONST.width, CONST.height);
        int iMax = CONST.width - CONST.bluringRay;
        int jMax = CONST.height - CONST.bluringRay;
        int bluringWindow = 1 + 2 * CONST.bluringRay;
        float bluringFactor = 1.0f / ((float) (bluringWindow * bluringWindow));
        Color [] windowColor;
        Color bluredColor;

        // Leave the side pixels empty
        if (CONST.width == CONST.height)
        {
            for(int i = 0; i < CONST.width; i++)
            {
                bluredTexture.SetPixel(i, 0, COLOR.empty);
                bluredTexture.SetPixel(i, jMax, COLOR.empty);
                bluredTexture.SetPixel(0, i, COLOR.empty);
                bluredTexture.SetPixel(iMax, i, COLOR.empty);
            }
        }
        else
        {
            for(int i = 0; i < CONST.width; i++)
            {
                bluredTexture.SetPixel(i, 0, COLOR.empty);
                bluredTexture.SetPixel(i, jMax, COLOR.empty);
            }

            for(int j = 0; j < CONST.height; j++)
            {
                bluredTexture.SetPixel(0, j, COLOR.empty);
                bluredTexture.SetPixel(jMax, j, COLOR.empty);
            }
        }

        // Blur pheromones within a given window
        for (int i = CONST.bluringRay; i < iMax; i++)
        {
            for (int j = CONST.bluringRay; j < jMax; j++)
            {
                bluredColor = COLOR.empty;

                // Diffuse pheromone
                windowColor = pheromoneTexture.GetPixels(i - 1, j - 1, bluringWindow, bluringWindow);

                for (int pixelIdx = 0; pixelIdx < windowColor.Length; pixelIdx++)
                {
                    bluredColor += windowColor[pixelIdx];
                }

                bluredColor.r *= bluringFactor;
                bluredColor.g *= bluringFactor;
                bluredColor.b *= bluringFactor;

                // Decay pheromones
                if (bluredColor != COLOR.empty)
                {
                    bluredColor.r = Mathf.Max(0.0f, bluredColor.r - CONST.frameEvaporation);
                    bluredColor.g = Mathf.Max(0.0f, bluredColor.g - CONST.frameEvaporation);
                    bluredColor.b = Mathf.Max(0.0f, bluredColor.b - CONST.frameEvaporation);
                }

                bluredTexture.SetPixel(i, j, bluredColor);
            }
        }

        // Store the blured texture
        pheromoneTexture = bluredTexture;
    }

    private void MoveAnts()
    {
        int antIdx;
        Ant ant;
        float directionAngle;
        float intentionAngle;
        float deltaAngle;
        float newDirectionAngle;

        for (antIdx = 0; antIdx < ants.Count; antIdx++)
        {
            ant = ants[antIdx];

            // Handle limit rebound if necessary
            BounceAnt(ref ant);

            // Make sure the ant intends to go home if it is carrying a resource
            if (ant.hasResource == true)
            {
                ant.intention = (CONST.homeCoordinates - ant.coordinates).normalized;
            }

            // Rotate ant if necessary
            if (ant.direction != ant.intention)
            {
                directionAngle = Mathf.Atan2(ant.direction.y, ant.direction.x);
                intentionAngle = Mathf.Atan2(ant.intention.y, ant.intention.x);

                deltaAngle = Mathf.Clamp(Mathf.DeltaAngle(directionAngle, intentionAngle),
                                         -CONST.antYawRate * CONST.deltaTime,
                                         CONST.antYawRate * CONST.deltaTime);

                newDirectionAngle = directionAngle + deltaAngle;

                ant.direction = new Vector2(Mathf.Cos(newDirectionAngle), Mathf.Sin(newDirectionAngle));
            }

            // Move ant
            ant.coordinates += ant.direction;

            // Pick up resource if the ant lands on it and has none yet
            if (    ( ant.hasResource                                           == false )
                 && ( resourceManager.GetResourceAtCoordinates(ant.coordinates) == true  ) )
            {
                ant.hasResource = true;

                resourceManager.RemoveResourceAtCoordinates(ant.coordinates);
            }

            // Drop resource and reset intention once brought back home
            if (    ( ant.hasResource                                     == true )
                 && ( (ant.coordinates - CONST.homeCoordinates).magnitude  < 1.0f ) )
            {
                ant.hasResource = false;

                ant.intention = new Vector2(Random.Range(-1.0f, 1.0f),
                                            Random.Range(-1.0f, 1.0f)).normalized;
            }

            if (ant.hasResource == true)
            {
                pheromoneTexture.SetPixel((int) ant.coordinates.x,
                                          (int) ant.coordinates.y,
                                          COLOR.pheroToHome);
            }
            else
            {
                pheromoneTexture.SetPixel((int) ant.coordinates.x,
                                          (int) ant.coordinates.y,
                                          COLOR.pheroToFood);
            }

            // Update ant
            ants[antIdx] = ant;
        }
    }

    public void UpdateAnts()
    {
        UpdatePheromones();

        MoveAnts();
    }

    public List<Vector2> GetAntsCoordinates()
    {
        int antIdx;
        List<Vector2> antsCoordinates = new List<Vector2>();

        for (antIdx = 0; antIdx < ants.Count; antIdx++)
        {
            antsCoordinates.Add(ants[antIdx].coordinates);
        }

        return antsCoordinates;
    }

    public Texture2D GetPheromoneTexture()
    {
        return pheromoneTexture;
    }
}
