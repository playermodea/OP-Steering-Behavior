using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenConfine : MonoBehaviour
{
    public Transform Tr;

    private void Awake()
    {
        Tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Tr.position.x > 43)
        {
            Tr.position = new Vector3(Tr.position.x * (-1) + 1, Tr.position.y, 0);
        }
        else if (Tr.position.x < -43)
        {
            Tr.position = new Vector3(Tr.position.x * (-1) - 1, Tr.position.y, 0);
        }
        if (Tr.position.y > 22)
        {
            Tr.position = new Vector3(Tr.position.x, Tr.position.y * (-1) + 1, 0);
        }
        else if (Tr.position.y < -22)
        {
            Tr.position = new Vector3(Tr.position.x, Tr.position.y * (-1) - 1, 0);
        }
    }
}
