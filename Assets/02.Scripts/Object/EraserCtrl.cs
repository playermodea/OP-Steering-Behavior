using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraserCtrl : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 1.0f);   
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Raiders")
            coll.GetComponent<RaidersCtrl>().Die();

        Destroy(gameObject);
    }
}
