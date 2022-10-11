using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raiders_Chase : MonoBehaviour, State
{
    private static Raiders_Chase instance;

    public static Raiders_Chase Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(Raiders_Chase)) as Raiders_Chase;
            }

            return instance;
        }
    }

    public virtual void Enter(GameObject owner)
    {
        var Owner = owner.GetComponent<RaidersCtrl>();

        MsgInfo Msg;
        Msg.Sender = owner;
        Msg.Msg = RaidersMsg.UpdatePlayerPos;

        EventManager.Instance.SendMessage("SendMsg", Msg);

        Owner.State = RaidersState.Chase;
        //Debug.Log("추적 상태 실행");
    }

    public virtual void Execute(GameObject owner)
    {
        var Owner = owner.GetComponent<RaidersCtrl>();
        var SB = owner.GetComponent<SteeringBehavior>();

        // 추격하기 조종행동으로 플레이어 추격
        Owner.CurrentSpeed += SB.Pursuit(Owner.Target);
    }

    public virtual void Exit(GameObject owner)
    {
        //Debug.Log("추적 상태 종료");
    }
}
