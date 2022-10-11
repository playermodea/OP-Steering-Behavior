using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    private static ObjectManager instance;

    public List<GameObject> RaidersList = new List<GameObject>();
    public List<GameObject> ObstacleList = new List<GameObject>();

    public static ObjectManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(ObjectManager)) as ObjectManager;
            }

            return instance;
        }
    }

    private void Awake()
    {
        foreach (GameObject Raiders in GameObject.FindGameObjectsWithTag("Raiders"))
        {
            RaidersList.Add(Raiders);
        }

        foreach (GameObject Obstacle in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            ObstacleList.Add(Obstacle);
        }
    }

    // 거리기준 오름차순 리스트 정렬
    public void AsendingSortingSortingList(Vector3 TargetPos, List<GameObject> SortingList)
    {
        for (int i = 0; i < SortingList.Count; i++)
        {
            float Distance_1 = (SortingList[i].transform.position - TargetPos).sqrMagnitude;
            for (int j = i + 1; j < SortingList.Count; j++)
            {
                float Distance_2 = (SortingList[j].transform.position - TargetPos).sqrMagnitude;
                if (Distance_1 > Distance_2)
                {
                    Swap(i, j, SortingList);
                    Distance_1 = (SortingList[i].transform.position - TargetPos).sqrMagnitude;
                }
            }
        }
    }

    public void Swap(int A, int B, List<GameObject> SortingList)
    {
        GameObject temp = SortingList[A];
        SortingList[A] = SortingList[B];
        SortingList[B] = temp;
    }
}
