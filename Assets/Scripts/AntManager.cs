using System.Collections.Generic;
using UnityEngine;

public class AntManager : MonoBehaviour
{
    public ResourceManager resourceManager;
    public int antQty;

    private List<Ant> ants = new List<Ant>();
    private Vector2 homeCoordinates = new Vector2();

    public void InitializeAnts()
    {
        for (int antIdx = 0 ; antIdx < antQty; antIdx++)
        {
            Ant newAnt = new Ant();

            newAnt.coordinates = new Vector2(Random.Range(0.0f, (float) CONST.width), Random.Range(0.0f, (float) CONST.height));
            newAnt.intention = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
            newAnt.direction = newAnt.intention;
            newAnt.hasResource = false;

            ants.Add(newAnt);
        }

        homeCoordinates.x = CONST.width / 2;
        homeCoordinates.y = CONST.height / 2;
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

    public void UpdateAnts()
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
                ant.intention = (homeCoordinates - ant.coordinates).normalized;
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
            if (    ( ant.hasResource                               == true )
                 && ( (ant.coordinates - homeCoordinates).magnitude  < 1.0f ) )
            {
                ant.hasResource = false;

                ant.intention = new Vector2(Random.Range(-1.0f, 1.0f),
                                            Random.Range(-1.0f, 1.0f)).normalized;
            }

            // Update ant
            ants[antIdx] = ant;
        }
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

    public Vector2Int GetHomeCoordinates()
    {
        return new Vector2Int((int) homeCoordinates.x, (int) homeCoordinates.y);
    }
}
