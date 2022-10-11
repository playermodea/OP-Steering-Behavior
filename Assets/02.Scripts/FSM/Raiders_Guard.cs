using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raiders_Guard : MonoBehaviour, State
{
    private static Raiders_Guard instance;

    public static Raiders_Guard Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(Raiders_Guard)) as Raiders_Guard;
            }

            return instance;
        }
    }

    public virtual void Enter(GameObject owner)
    {
        var Owner = owner.GetComponent<RaidersCtrl>();

        Owner.State = RaidersState.Guard;
        //Debug.Log("경계 상태 실행");
    }

    public virtual void Execute(GameObject owner)
    {
        var Owner = owner.GetComponent<RaidersCtrl>();
        var SB = owner.GetComponent<SteeringBehavior>();

        Owner.CurrentSpeed += SB.Wander();
        //Debug.Log("Raiders_Guard Execute 실행성공");
    }

    public virtual void Exit(GameObject owner)
    {
        //Debug.Log("경계 상태 종료");
    }
}
