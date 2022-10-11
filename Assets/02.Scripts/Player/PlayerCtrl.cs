using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCtrl : MonoBehaviour
{
    public Rigidbody TrRigidbody;

    public Transform Target;
    public SteeringBehavior SB;

    public Vector3 CurrentSpeed;

    private void Awake()
    {
        TrRigidbody = GetComponent<Rigidbody>();
        SB = GetComponent<SteeringBehavior>();
    }

    private void Start()
    {
        SB.MaxSpeed = 30.0f;
    }

    // Update is called once per frame
    void Update()
    {
        CurrentSpeed = Vector3.zero;
        CurrentSpeed += SB.Arrive(Target.position);
        TrRigidbody.AddForce(CurrentSpeed);

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }
}
