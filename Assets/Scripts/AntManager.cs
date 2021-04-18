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
            newAnt.direction = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
            newAnt.hasResource = false;

            ants.Add(newAnt);
        }

        homeCoordinates.x = CONST.width / 2;
        homeCoordinates.y = CONST.height / 2;
    }

    public void UpdateAnts()
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
            else if (ant.coordinates.x > ((float) CONST.width - 2.0f))
            {
                ant.direction.x = Random.Range(0.0f, -1.0f);
                ant.direction.Normalize();
            }

            if (ant.coordinates.y < 1.0f)
            {
                ant.direction.y = Random.Range(0.0f, 1.0f);
                ant.direction.Normalize();
            }
            else if (ant.coordinates.y > ((float) CONST.height - 2.0f))
            {
                ant.direction.y = Random.Range(0.0f, -1.0f);
                ant.direction.Normalize();
            }

            // Move then update ant
            ant.coordinates += ant.direction;

            // Set ant resource if it is located on a resource pixel
            if (resourceManager.GetResourceAtCoordinates(ant.coordinates) == true)
            {
                if (ant.hasResource == false)
                {
                    ant.hasResource = true;

                    ant.direction = (homeCoordinates - ant.coordinates).normalized;
                    resourceManager.RemoveResourceAtCoordinates(ant.coordinates);
                }
            }

            if ((ant.hasResource == true) && ((ant.coordinates - homeCoordinates).magnitude < 1.0f))
            {
                ant.hasResource = false;

                ant.direction = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
            }

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
