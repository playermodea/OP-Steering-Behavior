using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateText : MonoBehaviour
{
    private RaidersCtrl ParentCtrl;
    private TextMesh Text;

    private void Awake()
    {
        ParentCtrl = gameObject.transform.parent.gameObject.GetComponent<RaidersCtrl>();
        Text = GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        Text.text = ParentCtrl.State.ToString();
    }
}
