using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
    private const int LAST_LINE = 7;
    [SerializeField] GameObject[] ghosts;

    private HashSet<int> elemNumSet = new HashSet<int>();
    private int[] numList = new int[5] { 0, 1, 2, 3, 4 };

    public void SpawnGhostLine()
    {
        for (int i = 0; i < 5; i++)
        {
            Vector3 posToSpawn = new Vector3(LAST_LINE, 0.25f, i);
            if (Field.aliveTowers[i])
            {
                Instantiate(ghosts[GetFreeElement()], posToSpawn, Quaternion.identity, transform);
            }
        }

        elemNumSet.Clear();
    }

    public IEnumerator SpawnGhostWedge()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i == 0)
            {
                Vector3 posToSpawn = new Vector3(7, 0.25f, 2);
                if (Field.aliveTowers[2 ])
                {
                    Instantiate(ghosts[GetFreeElement()], posToSpawn, Quaternion.identity, transform);
                }

                yield return new WaitForSeconds(1.5f);
            }
            else
            {
                Vector3 posToSpawn_1 = new Vector3(7, 0.25f, 2 - i);
                Vector3 posToSpawn_2 = new Vector3(7, 0.25f, 2 + i);

                if (Field.aliveTowers[2 - i])
                {
                    Instantiate(ghosts[GetFreeElement()], posToSpawn_1, Quaternion.identity, transform);
                }
                if (Field.aliveTowers[2 + i])
                {
                    Instantiate(ghosts[GetFreeElement()], posToSpawn_2, Quaternion.identity, transform);
                }

                yield return new WaitForSeconds(1.5f);
            }

        }

        elemNumSet.Clear();
    }

    private int GetFreeElement()
    {
        int ghostNum = 0;
        bool exit = false;

        if (elemNumSet.Count == numList.Length - 1)
        {
            foreach (int i in numList)
            {
                if (!elemNumSet.Contains(i)) return i;
            }
        }
        else
        {
            while (!exit) // || elemNumSet.Count == numList.Length
            {
                ghostNum = numList[Random.Range(0, 5)];
                if (!elemNumSet.Contains(ghostNum))
                {
                    elemNumSet.Add(ghostNum);
                    exit = true;
                }
            }
        }

        return ghostNum;
    }
}
