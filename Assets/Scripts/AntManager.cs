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
            newAnt.nextRandomizationTimestamp = Time.time + Random.Range(ANT.randomizationPeriodMin, ANT.randomizationPeriodMax);

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

    public bool IsHome(int i, int j)
    {
        bool isHome = false;
        int iDiff = i - CONST.homeCoordinates.x;
        int jDiff = j - CONST.homeCoordinates.x;

        if (((iDiff * iDiff) + (jDiff * jDiff)) < (CONST.homeRadius * CONST.homeRadius))
        {
            isHome = true;
        }

        return isHome;
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
        if (CONST.bluringRay > 0)
        {
            for (int i = CONST.bluringRay; i < iMax; i++)
            {
                for (int j = CONST.bluringRay; j < jMax; j++)
                {
                    bluredColor = COLOR.empty;

                    windowColor = pheromoneTexture.GetPixels(i - 1, j - 1, bluringWindow, bluringWindow);

                    for (int pixelIdx = 0; pixelIdx < windowColor.Length; pixelIdx++)
                    {
                        bluredColor += windowColor[pixelIdx];
                    }

                    if (bluredColor != COLOR.empty)
                    {
                        // Diffuse pheromone
                        bluredColor.r *= bluringFactor;
                        bluredColor.g *= bluringFactor;
                        bluredColor.b *= bluringFactor;

                        // Decay pheromone
                        bluredColor.r = Mathf.Max(0.0f, bluredColor.r - CONST.frameEvaporation);
                        bluredColor.g = Mathf.Max(0.0f, bluredColor.g - CONST.frameEvaporation);
                        bluredColor.b = Mathf.Max(0.0f, bluredColor.b - CONST.frameEvaporation);
                    }

                    bluredTexture.SetPixel(i, j, bluredColor);
                }
            }
        }
        else
        {
            for (int i = CONST.bluringRay; i < iMax; i++)
            {
                for (int j = CONST.bluringRay; j < jMax; j++)
                {
                    bluredColor = pheromoneTexture.GetPixel(i, j);

                    if (bluredColor != COLOR.empty)
                    {
                        // Decay pheromone
                        bluredColor.r = Mathf.Max(0.0f, bluredColor.r - CONST.frameEvaporation);
                        bluredColor.g = Mathf.Max(0.0f, bluredColor.g - CONST.frameEvaporation);
                        bluredColor.b = Mathf.Max(0.0f, bluredColor.b - CONST.frameEvaporation);
                    }

                    bluredTexture.SetPixel(i, j, bluredColor);
                }
            }
        }

        // Store the blured texture
        pheromoneTexture = bluredTexture;
    }

    private void ScanFieldOfView(ref Ant ant)
    {
        Texture2D resourceTexture = resourceManager.GetResourcesTexture();
        List<Vector2Int> pixelsInFov = new List<Vector2Int>();
        int iAnt = (int) ant.coordinates.x;
        int jAnt = (int) ant.coordinates.y;
        int iMin = Mathf.Min(iAnt - ANT.fovRange, 0);
        int iMax = Mathf.Max(iAnt + ANT.fovRange + 1, CONST.width);
        int jMin = Mathf.Min(jAnt - ANT.fovRange, 0);
        int jMax = Mathf.Max(jAnt + ANT.fovRange + 1, CONST.height);
        float antDirection = Vector2.SignedAngle(Vector2.right, ant.direction);
        float fovMinAngle = (antDirection - ANT.fovAngleHalf) * Mathf.Deg2Rad;
        float fovMaxAngle = (antDirection + ANT.fovAngleHalf) * Mathf.Deg2Rad;
        Vector2 fovMinEdge = ant.coordinates + ANT.fovRange * (new Vector2(Mathf.Cos(fovMinAngle), Mathf.Sin(fovMinAngle)));
        Vector2 fovMaxEdge = ant.coordinates + ANT.fovRange * (new Vector2(Mathf.Cos(fovMaxAngle), Mathf.Sin(fovMaxAngle)));
        bool isTargetFound;
        bool isPheromoneFound;
        float oldestPheromoneStrength;
        float pheromoneStrength;
        Vector2 newIntentionCoordinates = new Vector2();

        // Reduce window to the field of view
        iMin = Mathf.Max(iMin,
                         Mathf.Min((int) fovMinEdge.x,
                                   (int) fovMaxEdge.x));
        iMax = Mathf.Min(iMax,
                         Mathf.Max((int) fovMinEdge.x,
                                   (int) fovMaxEdge.x));
        jMin = Mathf.Max(jMin,
                         Mathf.Min((int) fovMinEdge.y,
                                   (int) fovMaxEdge.y));
        jMax = Mathf.Min(jMax,
                         Mathf.Max((int) fovMinEdge.y,
                                   (int) fovMaxEdge.y));

        // Initialize oldest pheromone
        isPheromoneFound = false;
        oldestPheromoneStrength = 1.0f;

        // Initialize prioritary target status
        isTargetFound = false;

        for (int i = iMin; (i < iMax) && (isTargetFound == false); i++)
        {
            for (int j = jMin; (j < jMax) && (isTargetFound == false); j++)
            {
                if (    ( i != iAnt )
                     || ( j != jAnt ) )
                {
                    if (ant.hasResource == false)
                    {
                        if (resourceTexture.GetPixel(i, j) == COLOR.resource)
                        {
                            // Prioritary target found
                            isTargetFound = true;

                            // Store resource coordinates
                            newIntentionCoordinates.x = (float) i;
                            newIntentionCoordinates.y = (float) j;

                            // Stop looking further
                            break;
                        }
                        else
                        {
                            // Look for home-going trails
                            pheromoneStrength = pheromoneTexture.GetPixel(i, j).b;
                        }
                    }
                    else
                    {
                        if (IsHome(i, j) == true)
                        {
                            // Prioritary target found
                            isTargetFound = true;

                            // Store home coordinates
                            newIntentionCoordinates = CONST.homeCoordinates;

                            // Stop looking further
                            break;
                        }
                        else
                        {
                            // Look for food-searching trails
                            pheromoneStrength = pheromoneTexture.GetPixel(i, j).r;
                        }
                    }

                    if (    ( pheromoneStrength > ANT.minPheromoneStrength )
                         && ( pheromoneStrength < oldestPheromoneStrength  ) )
                    {
                        isPheromoneFound = true;

                        // Store oldest pheromone in field of view coordinates
                        newIntentionCoordinates.x = (float) i;
                        newIntentionCoordinates.y = (float) j;
                    }
                }
            }
        }

        if (    ( isTargetFound    == true )
             || ( isPheromoneFound == true ) )
        {
            // Intend going towards the oldest pheromone in field of view
            ant.intention = (newIntentionCoordinates - ant.coordinates).normalized;
        }
    }

    private void UpdateAntIntention(ref Ant ant)
    {
        ScanFieldOfView(ref ant);

//        if (Time.time > ant.nextRandomizationTimestamp)
//        {
//            // Randomize intention
//            ant.intention = Random.insideUnitCircle.normalized;
//
//            // Randomize next randomization timestamp
//            ant.nextRandomizationTimestamp = Time.time + Random.Range(ANT.randomizationPeriodMin, ANT.randomizationPeriodMax);
//        }
    }

    private void MoveAnts()
    {
        int antIdx;
        Ant ant;
        float deltaAngle;
        float newDirectionAngle;

        for (antIdx = 0; antIdx < ants.Count; antIdx++)
        {
            ant = ants[antIdx];

            // Handle limit rebound if necessary
            BounceAnt(ref ant);

            // Update ant intention if necessary
            UpdateAntIntention(ref ant);

            // Rotate ant if necessary
            deltaAngle = Vector2.SignedAngle(ant.direction, ant.intention);

            if (Mathf.Abs(deltaAngle) > CONST.almostZero)
            {
                deltaAngle = Mathf.Clamp(deltaAngle,
                                         -ANT.yawRate * CONST.deltaTime,
                                         ANT.yawRate * CONST.deltaTime);

                newDirectionAngle = (Vector2.SignedAngle(Vector2.right, ant.direction) + deltaAngle) * Mathf.Deg2Rad;

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
