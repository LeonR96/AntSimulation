using System.Collections.Generic;
using UnityEngine;

public class AntManager : MonoBehaviour
{
    public int antQty;

    private List<Ant> ants = new List<Ant>();

    public void InitializeAnts()
    {
        for (int antIdx = 0 ; antIdx < antQty; antIdx++)
        {
            Ant newAnt = new Ant();

            newAnt.coordinates = new Vector2(Random.Range(0.0f, (float) CONST.width), Random.Range(0.0f, (float) CONST.height));
            newAnt.direction = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;

            ants.Add(newAnt);
        }
    }

    public void UpdateAnts(Texture2D texture)
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
            ants[antIdx] = ant;

            // Draw ant
            texture.SetPixel((int) ant.coordinates.x, (int) ant.coordinates.y, CONST.antColor);
        }

        texture.Apply();
    }
}
