using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAgent : MonoBehaviour
{
    private Transform Tr;
    private Rigidbody TrRigidbody;

    private void Awake()
    {
        Tr = GetComponent<Transform>();
        TrRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TrRigidbody.velocity != Vector3.zero)
        {
            TrRigidbody.freezeRotation = false;
            Vector3 Dir = TrRigidbody.velocity;
            float Angle = Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg;
            Tr.rotation = Quaternion.AngleAxis(Angle, Vector3.forward);
        }
        else
        {
            TrRigidbody.freezeRotation = true;
        }
    }
}
