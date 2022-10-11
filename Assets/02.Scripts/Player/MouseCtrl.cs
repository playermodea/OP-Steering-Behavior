using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCtrl : MonoBehaviour
{
    public Vector2 MousePos;
    public Camera Cam;
    public GameObject Eraser;

    // Start is called before the first frame update
    void Start()
    {
        Cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        MousePos = Input.mousePosition;
        MousePos = Cam.ScreenToWorldPoint(MousePos);

        if (Input.GetMouseButtonDown(0))
        {
            transform.position = MousePos;
        }
        if (Input.GetMouseButtonDown(1))
        {
            Instantiate(Eraser, new Vector3(MousePos.x, MousePos.y, 0), Quaternion.identity);
        }
    }
}
