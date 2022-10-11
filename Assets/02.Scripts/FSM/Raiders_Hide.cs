using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raiders_Hide : MonoBehaviour, State
{
    private static Raiders_Hide instance;

    public static Raiders_Hide Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(Raiders_Hide)) as Raiders_Hide;
            }

            return instance;
        }
    }

    public virtual void Enter(GameObject owner)
    {
        var Owner = owner.GetComponent<RaidersCtrl>();

        Owner.State = RaidersState.Hide;
        //Debug.Log("숨기 상태 실행");
    }

    public virtual void Execute(GameObject owner)
    {
        var Owner = owner.GetComponent<RaidersCtrl>();
        var SB = owner.GetComponent<SteeringBehavior>();

        // 만약 혼자 남았다면 플레이어 에게서 계속 도망칠 수 있도록 Evade 상태로 전환
        if (ObjectManager.Instance.RaidersList.Count == 1)
        {
            Owner.GetSM().ChangeState(Raiders_Evade.Instance);
        }

        // Hide 상태는 보스만 돌입
        //플레이어를 발견했다고 보고받은 보스는 현재 위치 기준 가장 가까운 장애물 뒤로 숨음, 주변에 장애물이 없으면 Evade 조종행동 발동
        Owner.CurrentSpeed += SB.Hide(Owner.Target, ObjectManager.Instance.ObstacleList);
    }

    public virtual void Exit(GameObject owner)
    {
        //Debug.Log("숨기 상태 종료");
    }
}
