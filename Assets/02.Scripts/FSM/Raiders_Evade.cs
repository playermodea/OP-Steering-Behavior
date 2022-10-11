using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raiders_Evade : MonoBehaviour, State
{
    private static Raiders_Evade instance;

    public static Raiders_Evade Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(Raiders_Evade)) as Raiders_Evade;
            }

            return instance;
        }
    }

    public virtual void Enter(GameObject owner)
    {
        var Owner = owner.GetComponent<RaidersCtrl>();

        Owner.State = RaidersState.Evade;
        //Debug.Log("도망 상태 실행");
    }

    public virtual void Execute(GameObject owner)
    {
        var Owner = owner.GetComponent<RaidersCtrl>();
        var SB = owner.GetComponent<SteeringBehavior>();

        // 도망가기 조종행동으로 플레이어에게서 도망치기
        Owner.CurrentSpeed += SB.Evade(Owner.Target);
    }

    public virtual void Exit(GameObject owner)
    {
        //Debug.Log("도망 상태 종료");
    }
}
