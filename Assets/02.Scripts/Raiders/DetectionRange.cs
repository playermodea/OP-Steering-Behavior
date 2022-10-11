using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionRange : MonoBehaviour
{
    private RaidersCtrl ParentCtrl;

    public bool IsDetection;
    public int DetectCount;
    public int DetectLimit;
    public float DetectTime;
    public float DetectTimeDelay;
    public float BackToGuardTime;
    public float BackToGuardTimeDelay;

    private void Awake()
    {
        ParentCtrl = gameObject.transform.parent.gameObject.GetComponent<RaidersCtrl>();

        IsDetection = true;
        DetectCount = 0;
        DetectLimit = 2;
        DetectTime = 0.0f;
        DetectTimeDelay = 2.0f;
        BackToGuardTime = 0.0f;
        BackToGuardTimeDelay = 6.0f;
    }

    private void Update()
    {
        if (ParentCtrl.State == RaidersState.Detect)
        {
            DetectTime += Time.deltaTime;
            BackToGuardTime += Time.deltaTime;

            if (DetectTime > DetectTimeDelay)
            {
                IsDetection = true;
                DetectTime = 0.0f;
            }
        }
    }

    void OnTriggerStay(Collider coll)
    {
        Transform Tr = ParentCtrl.GetTr();
        float RayLength = 20.0f;

        RaycastHit Hit;
        Vector3 Direction = (coll.transform.position - Tr.position).normalized;
        int Mask = 1 << 9;
        Mask = ~Mask;

        if (Physics.Raycast(Tr.position, Direction, out Hit, RayLength, Mask))
        {
            if (Hit.collider.tag == "Player")
            {
                ParentCtrl.Target = Hit.collider.transform;

                if (ParentCtrl.State == RaidersState.Guard)
                {
                    IsDetection = false;
                    ParentCtrl.Target = Hit.collider.transform;
                    ParentCtrl.TargetPos = Hit.collider.transform.position;
                    ParentCtrl.GetSM().ChangeState(Raiders_Detect.Instance);
                    //Debug.Log("플레이어 감지");
                }
                if (ParentCtrl.State == RaidersState.Detect)
                {
                    ParentCtrl.TargetPos = Hit.collider.transform.position;

                    if (IsDetection == true)
                    {
                        DetectCount++;
                        IsDetection = false;
                    }
                }
                if (ParentCtrl.State == RaidersState.Wait)
                {
                    ParentCtrl.TargetPos = Hit.collider.transform.position;
                }
            }
            Debug.DrawRay(Tr.position, Direction * RayLength, Color.blue, 0.3f);
        }
    }
}
