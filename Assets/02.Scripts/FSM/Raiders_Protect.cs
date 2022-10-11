using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raiders_Protect : MonoBehaviour, State
{
    private static Raiders_Protect instance;

    public static Raiders_Protect Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(Raiders_Protect)) as Raiders_Protect;
            }

            return instance;
        }
    }

    public virtual void Enter(GameObject owner)
    {
        var Owner = owner.GetComponent<RaidersCtrl>();

        Owner.State = RaidersState.Protect;
        //Debug.Log("보호 상태 실행");
    }

    public virtual void Execute(GameObject owner)
    {
        var Owner = owner.GetComponent<RaidersCtrl>();
        var SB = owner.GetComponent<SteeringBehavior>();

        // 보스 주변 위치로 Offset 추격하기 조종행동으로 보스 보호하기 위해 이동
        Owner.CurrentSpeed += SB.OffsetPursuit(Owner.OffsetTarget, Owner.Offset);
        Vector3 TargetPos = Owner.TargetPos;
    }

    public virtual void Exit(GameObject owner)
    {
        //Debug.Log("보호 상태 종료");
    }
}
